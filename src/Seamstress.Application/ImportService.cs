using System.Collections.Concurrent;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
    public class NormalizedProduct
    {
        public string ExternalId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Fabric { get; set; } = null!;
        public List<string> Colors { get; set; } = new();
        public List<string> Sizes { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
        public string MeasurementsDescription { get; set; } = "";
    }

    public class ImportService : IImportService
    {
        private readonly IGeneralPersistence _generalPersistence;
        private readonly IItemPersistence _itemPersistence;
        private readonly IColorPersistence _colorPersistence;
        private readonly IFabricPersistence _fabricPersistence;
        private readonly ISizePersistence _sizePersistence;
        private readonly ImageProcessingQueue _imageQueue;

        // In-memory session cache: sessionId -> session data
        private static readonly ConcurrentDictionary<string, ImportSession> _sessions = new();

        public ImportService(
            IGeneralPersistence generalPersistence,
            IItemPersistence itemPersistence,
            IColorPersistence colorPersistence,
            IFabricPersistence fabricPersistence,
            ISizePersistence sizePersistence,
            ImageProcessingQueue imageQueue)
        {
            _generalPersistence = generalPersistence;
            _itemPersistence = itemPersistence;
            _colorPersistence = colorPersistence;
            _fabricPersistence = fabricPersistence;
            _sizePersistence = sizePersistence;
            _imageQueue = imageQueue;
        }

        public async Task<ImportPreviewDto> GeneratePreviewAsync(List<NormalizedProduct> products, int salePlatformId)
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString();

                // Clean up expired sessions
                CleanExpiredSessions();

                // Step 1: Validate fabrics - must exist in DB
                var allFabrics = await _fabricPersistence.GetAllFabricsAsync();
                var validProducts = new List<NormalizedProduct>();
                var failedProducts = new List<ImportPreviewItemDto>();

                foreach (var product in products)
                {
                    var fabricMatch = allFabrics.FirstOrDefault(f =>
                        string.Equals(f.Name?.Trim(), product.Fabric?.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (fabricMatch == null && !string.IsNullOrWhiteSpace(product.Fabric))
                    {
                        failedProducts.Add(new ImportPreviewItemDto
                        {
                            ExternalId = product.ExternalId,
                            Name = product.Name,
                            Price = product.Price,
                            Fabric = product.Fabric,
                            Colors = product.Colors,
                            Sizes = product.Sizes,
                            Action = "Failed",
                            FailReason = $"Tecido não encontrado: {product.Fabric}"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(product.Fabric))
                    {
                        failedProducts.Add(new ImportPreviewItemDto
                        {
                            ExternalId = product.ExternalId,
                            Name = product.Name,
                            Price = product.Price,
                            Fabric = "",
                            Colors = product.Colors,
                            Sizes = product.Sizes,
                            Action = "Failed",
                            FailReason = "Nenhuma categoria de tecido encontrada"
                        });
                        continue;
                    }

                    validProducts.Add(product);
                }

                // Step 2: Diff against existing items
                var existingItems = await _itemPersistence.GetItemsByExternalSourceAsync(salePlatformId);
                var existingDict = existingItems
                    .Where(i => i.ExternalId != null)
                    .ToDictionary(i => i.ExternalId!);

                var productExternalIds = validProducts.Select(r => r.ExternalId).ToHashSet();

                var preview = new ImportPreviewDto { SessionId = sessionId, Failed = failedProducts };

                // Classify: Create or Update
                foreach (var row in validProducts)
                {
                    if (existingDict.TryGetValue(row.ExternalId, out var existing))
                    {
                        var changes = DetectChanges(existing, row);
                        if (changes.Count > 0)
                        {
                            preview.ToUpdate.Add(new ImportPreviewItemDto
                            {
                                ExternalId = row.ExternalId,
                                Name = row.Name,
                                Price = row.Price,
                                Fabric = row.Fabric,
                                Colors = row.Colors,
                                Sizes = row.Sizes,
                                Action = "Update",
                                Changes = changes
                            });
                        }
                    }
                    else
                    {
                        preview.ToCreate.Add(new ImportPreviewItemDto
                        {
                            ExternalId = row.ExternalId,
                            Name = row.Name,
                            Price = row.Price,
                            Fabric = row.Fabric,
                            Colors = row.Colors,
                            Sizes = row.Sizes,
                            Action = "Create"
                        });
                    }
                }

                // Classify: Inactivate (in DB but not in incoming products)
                foreach (var existing in existingItems.Where(i => i.IsActive == true && i.ExternalId != null))
                {
                    if (!productExternalIds.Contains(existing.ExternalId!))
                    {
                        preview.ToInactivate.Add(new ImportPreviewItemDto
                        {
                            ExternalId = existing.ExternalId!,
                            Name = existing.Name,
                            Price = existing.Price,
                            Fabric = existing.ItemFabrics.FirstOrDefault()?.Fabric?.Name ?? "",
                            Colors = existing.ItemColors.Select(ic => ic.Color?.Name ?? "").ToList(),
                            Sizes = existing.ItemSizes.Select(isz => isz.Size?.Name ?? "").ToList(),
                            Action = "Inactivate"
                        });
                    }
                }

                // Store session with preview + products for execute step
                _sessions[sessionId] = new ImportSession
                {
                    SalePlatformId = salePlatformId,
                    Preview = preview,
                    Products = validProducts,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                };

                return preview;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao gerar preview: {ex.Message}");
            }
        }

        public async Task<ImportResultDto> ExecuteImportAsync(string sessionId)
        {
            var result = new ImportResultDto();

            try
            {
                var session = GetSession(sessionId);
                if (session.Preview == null)
                    throw new Exception("Preview não gerado. Execute o preview antes de importar.");

                // Carry over failed imports from preview
                result.Failed = session.Preview.Failed;

                var preview = session.Preview;

                // Load all lookup data for entity resolution
                var allColors = (await _colorPersistence.GetAllColorsAsync()).ToList();
                var allFabrics = (await _fabricPersistence.GetAllFabricsAsync()).ToList();
                var allSizes = (await _sizePersistence.GetAllSizesAsync()).ToList();

                // Build normalized products map from session products
                var normalizedProducts = session.Products.ToDictionary(p => p.ExternalId);

                // PRE-RESOLVE all entities before transaction (batch, not N+1)
                var allColorNames = normalizedProducts.Values.SelectMany(p => p.Colors).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var allFabricNames = normalizedProducts.Values.Select(p => p.Fabric).Where(f => !string.IsNullOrWhiteSpace(f)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var allSizeNames = normalizedProducts.Values.SelectMany(p => p.Sizes).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var colorMap = await ResolveEntitiesBatch(allColorNames, allColors, _colorPersistence.GetAllColorsAsync, (name) => new Color { Name = name, IsActive = true });
                var fabricMap = await ResolveEntitiesBatch(allFabricNames, allFabrics, _fabricPersistence.GetAllFabricsAsync, (name) => new Fabric { Name = name, IsActive = true });
                var sizeMap = await ResolveEntitiesBatch(allSizeNames, allSizes, _sizePersistence.GetAllSizesAsync, (name) => new Size { Name = name, IsActive = true });

                _generalPersistence.BeginTransaction();

                try
                {
                    // CREATE new items
                    foreach (var createItem in preview.ToCreate)
                    {
                        var product = normalizedProducts[createItem.ExternalId];
                        var item = new Item
                        {
                            Name = product.Name,
                            Price = product.Price,
                            ExternalId = product.ExternalId,
                            SalePlatformId = session.SalePlatformId,
                            IsActive = true,
                            ImageURL = "",
                            MeasurementsDescription = product.MeasurementsDescription
                        };

                        item.ItemColors = product.Colors
                            .Where(c => colorMap.ContainsKey(c.Trim().ToLower()))
                            .Select(c => new ItemColor { ColorId = colorMap[c.Trim().ToLower()] }).ToList();
                        item.ItemFabrics = string.IsNullOrWhiteSpace(product.Fabric) ? new List<ItemFabric>()
                            : new List<ItemFabric> { new ItemFabric { FabricId = fabricMap[product.Fabric.Trim().ToLower()] } };
                        item.ItemSizes = product.Sizes
                            .Where(s => sizeMap.ContainsKey(s.Trim().ToLower()))
                            .Select(s => new ItemSize { SizeId = sizeMap[s.Trim().ToLower()] }).ToList();

                        _generalPersistence.Add(item);
                        result.Created++;
                    }

                    await _generalPersistence.SaveChangesAsync();

                    // UPDATE existing items - re-fetch tracked via GetItemByIdTrackedAsync
                    var existingItems = await _itemPersistence.GetItemsByExternalSourceAsync(session.SalePlatformId);
                    var existingDict = existingItems.Where(i => i.ExternalId != null).ToDictionary(i => i.ExternalId!);

                    foreach (var updateItem in preview.ToUpdate)
                    {
                        if (!existingDict.TryGetValue(updateItem.ExternalId, out var existing)) continue;
                        var product = normalizedProducts[updateItem.ExternalId];

                        // Re-fetch as tracked entity
                        var tracked = await _itemPersistence.GetItemByIdTrackedAsync(existing.Id);
                        if (tracked == null) continue;

                        tracked.Name = product.Name;
                        tracked.Price = product.Price;
                        tracked.MeasurementsDescription = product.MeasurementsDescription;

                        // Update colors: remove old, add new
                        var existingColorIds = tracked.ItemColors.Select(ic => ic.ColorId).ToHashSet();
                        var newColorIds = product.Colors.Where(c => colorMap.ContainsKey(c.Trim().ToLower())).Select(c => colorMap[c.Trim().ToLower()]).ToHashSet();

                        var colorsToRemove = tracked.ItemColors.Where(ic => !newColorIds.Contains(ic.ColorId)).ToArray();
                        if (colorsToRemove.Length > 0) _generalPersistence.DeleteRange(colorsToRemove);
                        foreach (var colorId in newColorIds.Where(id => !existingColorIds.Contains(id)))
                            _generalPersistence.Add(new ItemColor { ItemId = tracked.Id, ColorId = colorId });

                        // Update fabrics: remove old, add new
                        var existingFabricIds = tracked.ItemFabrics.Select(f => f.FabricId).ToHashSet();
                        var newFabricIds = string.IsNullOrWhiteSpace(product.Fabric) ? new HashSet<int>()
                            : new HashSet<int> { fabricMap[product.Fabric.Trim().ToLower()] };

                        var fabricsToRemove = tracked.ItemFabrics.Where(f => !newFabricIds.Contains(f.FabricId)).ToArray();
                        if (fabricsToRemove.Length > 0) _generalPersistence.DeleteRange(fabricsToRemove);
                        foreach (var fabricId in newFabricIds.Where(id => !existingFabricIds.Contains(id)))
                            _generalPersistence.Add(new ItemFabric { ItemId = tracked.Id, FabricId = fabricId });

                        // Update sizes: only add new, remove stale. NEVER recreate existing (preserves measurements)
                        var existingSizeIds = tracked.ItemSizes.Select(s => s.SizeId).ToHashSet();
                        var newSizeIds = product.Sizes.Where(s => sizeMap.ContainsKey(s.Trim().ToLower())).Select(s => sizeMap[s.Trim().ToLower()]).ToHashSet();

                        var sizesToRemove = tracked.ItemSizes.Where(s => !newSizeIds.Contains(s.SizeId)).ToArray();
                        if (sizesToRemove.Length > 0) _generalPersistence.DeleteRange(sizesToRemove);
                        foreach (var sizeId in newSizeIds.Where(id => !existingSizeIds.Contains(id)))
                            _generalPersistence.Add(new ItemSize { ItemId = tracked.Id, SizeId = sizeId });

                        // tracked is loaded with change tracking enabled; EF auto-detects scalar mutations
                        // and persists the explicit Add/DeleteRange on nav children. Calling DbSet.Update here
                        // would re-traverse the graph and revert the Deleted/Added child states.
                        result.Updated++;
                    }

                    await _generalPersistence.SaveChangesAsync();

                    // INACTIVATE items no longer in incoming products
                    foreach (var inactivateItem in preview.ToInactivate)
                    {
                        if (!existingDict.TryGetValue(inactivateItem.ExternalId, out var existing)) continue;
                        var tracked = await _itemPersistence.GetItemByIdTrackedAsync(existing.Id);
                        if (tracked == null) continue;

                        tracked.IsActive = false;
                        _generalPersistence.Update(tracked);
                        result.Inactivated++;
                    }

                    await _generalPersistence.SaveChangesAsync();
                    _generalPersistence.CommitTransaction();
                }
                catch
                {
                    _generalPersistence.RollbackTransaction();
                    throw;
                }

                // Clear change tracker
                _generalPersistence.ClearChangeTracker();

                // Enqueue image processing only for created/updated items
                var changedIds = new HashSet<string>();
                foreach (var item in preview.ToCreate) changedIds.Add(item.ExternalId);
                foreach (var item in preview.ToUpdate) changedIds.Add(item.ExternalId);

                if (changedIds.Count > 0)
                {
                    await _imageQueue.EnqueueAsync(new ImageProcessingJob
                    {
                        Products = normalizedProducts,
                        ChangedExternalIds = changedIds,
                        SalePlatformId = session.SalePlatformId
                    });
                }

                // Clean up session
                _sessions.TryRemove(sessionId, out _);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar importação: {ex.Message}");
            }
        }

        #region Private Helper Methods

        private void CleanExpiredSessions()
        {
            var expired = _sessions.Where(s => s.Value.ExpiresAt < DateTime.UtcNow).Select(s => s.Key).ToList();
            foreach (var key in expired) _sessions.TryRemove(key, out _);
        }

        private ImportSession GetSession(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session) || session.ExpiresAt < DateTime.UtcNow)
            {
                _sessions.TryRemove(sessionId, out _);
                throw new Exception("Sessão de importação expirada ou não encontrada.");
            }
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(30); // refresh
            return session;
        }

        private List<string> DetectChanges(Item existing, NormalizedProduct incoming)
        {
            var changes = new List<string>();

            if (existing.Name != incoming.Name)
                changes.Add($"Nome: {existing.Name} → {incoming.Name}");
            if (existing.Price != incoming.Price)
                changes.Add($"Preço: {existing.Price} → {incoming.Price}");

            var existingColors = existing.ItemColors.Select(ic => ic.Color?.Name?.Trim().ToLower()).OrderBy(c => c).ToList();
            var incomingColors = incoming.Colors.Select(c => c.Trim().ToLower()).OrderBy(c => c).ToList();
            if (!existingColors.SequenceEqual(incomingColors))
                changes.Add($"Cores: [{string.Join(", ", existingColors)}] → [{string.Join(", ", incomingColors)}]");

            var existingFabric = existing.ItemFabrics.FirstOrDefault()?.Fabric?.Name?.Trim().ToLower() ?? "";
            if (existingFabric != incoming.Fabric.Trim().ToLower())
                changes.Add($"Tecido: {existingFabric} → {incoming.Fabric}");

            var existingSizes = existing.ItemSizes.Select(isz => isz.Size?.Name?.Trim().ToLower()).OrderBy(s => s).ToList();
            var incomingSizes = incoming.Sizes.Select(s => s.Trim().ToLower()).OrderBy(s => s).ToList();
            if (!existingSizes.SequenceEqual(incomingSizes))
                changes.Add($"Tamanhos: [{string.Join(", ", existingSizes)}] → [{string.Join(", ", incomingSizes)}]");

            var existingDesc = existing.MeasurementsDescription ?? "";
            var incomingDesc = incoming.MeasurementsDescription ?? "";
            if (existingDesc != incomingDesc)
                changes.Add($"Descrição: atualizada");

            return changes;
        }

        /// <summary>
        /// Batch resolve entities: match by name (case-insensitive), create if missing, reactivate if inactive.
        /// Returns a dictionary of lowercase name -> entity ID.
        /// </summary>
        private async Task<Dictionary<string, int>> ResolveEntitiesBatch<T>(
            List<string> names,
            List<T> existingEntities,
            Func<Task<T[]>> refreshFunc,
            Func<string, T> createFunc) where T : class
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var needsSave = false;

            // Normalize input: trim to align with GetName (which also trims) so case-insensitive
            // matching is not defeated by stray whitespace from upstream sources (e.g. NuvemShop).
            var trimmedNames = names.Select(n => n.Trim()).Where(n => n.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var name in trimmedNames)
            {
                var existing = existingEntities.FirstOrDefault(e =>
                    string.Equals(GetName(e), name, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    var newEntity = createFunc(name);
                    _generalPersistence.Add(newEntity);
                    needsSave = true;
                }
                else
                {
                    var isActive = GetIsActive(existing);
                    if (isActive != true)
                    {
                        SetIsActive(existing, true);
                        _generalPersistence.Update(existing);
                        needsSave = true;
                    }
                    result[name.ToLower()] = GetId(existing);
                }
            }

            if (needsSave)
            {
                await _generalPersistence.SaveChangesAsync();
                // Refresh to get IDs for newly created entities
                var refreshed = await refreshFunc();
                foreach (var name in trimmedNames)
                {
                    var entity = refreshed.FirstOrDefault(e =>
                        string.Equals(GetName(e), name, StringComparison.OrdinalIgnoreCase));
                    if (entity != null)
                        result[name.ToLower()] = GetId(entity);
                }
            }

            return result;
        }

        // Simple reflection helpers for the shared entity shape (Id, Name, IsActive)
        private static string GetName<T>(T entity) => ((string)typeof(T).GetProperty("Name")!.GetValue(entity)!).Trim();
        private static int GetId<T>(T entity) => (int)typeof(T).GetProperty("Id")!.GetValue(entity)!;
        private static bool? GetIsActive<T>(T entity) => (bool?)typeof(T).GetProperty("IsActive")?.GetValue(entity);
        private static void SetIsActive<T>(T entity, bool value) => typeof(T).GetProperty("IsActive")?.SetValue(entity, (bool?)value);

        #endregion

        #region Private Classes

        private class ImportSession
        {
            public int SalePlatformId { get; set; }
            public ImportPreviewDto? Preview { get; set; }
            public List<NormalizedProduct> Products { get; set; } = new();
            public DateTime ExpiresAt { get; set; }
        }

        #endregion
    }
}
