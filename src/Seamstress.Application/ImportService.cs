using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
    public class ImportService : IImportService
    {
        private readonly IGeneralPersistence _generalPersistence;
        private readonly IItemPersistence _itemPersistence;
        private readonly IImportMappingPersistence _importMappingPersistence;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IColorPersistence _colorPersistence;
        private readonly IFabricPersistence _fabricPersistence;
        private readonly ISizePersistence _sizePersistence;
        private readonly HttpClient _httpClient;

        // In-memory session cache: sessionId -> (parsedRows, previewData, expiry)
        private static readonly ConcurrentDictionary<string, ImportSession> _sessions = new();

        public ImportService(
            IGeneralPersistence generalPersistence,
            IItemPersistence itemPersistence,
            IImportMappingPersistence importMappingPersistence,
            IAzureBlobService azureBlobService,
            IColorPersistence colorPersistence,
            IFabricPersistence fabricPersistence,
            ISizePersistence sizePersistence,
            HttpClient httpClient)
        {
            _generalPersistence = generalPersistence;
            _itemPersistence = itemPersistence;
            _importMappingPersistence = importMappingPersistence;
            _azureBlobService = azureBlobService;
            _colorPersistence = colorPersistence;
            _fabricPersistence = fabricPersistence;
            _sizePersistence = sizePersistence;
            _httpClient = httpClient;
        }

        public async Task<ImportUploadResultDto> ParseCsvAsync(IFormFile file, int salePlatformId)
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString();
                var rows = new List<Dictionary<string, string>>();

                // NuvemShop CSV uses semicolon delimiter and latin-1 encoding
                using var reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.Latin1);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                });

                await csv.ReadAsync();
                csv.ReadHeader();
                var headers = csv.HeaderRecord?.ToList() ?? new List<string>();

                while (await csv.ReadAsync())
                {
                    var row = new Dictionary<string, string>();
                    foreach (var header in headers)
                    {
                        row[header] = csv.GetField(header) ?? "";
                    }
                    rows.Add(row);
                }

                // Clean up expired sessions
                CleanExpiredSessions();

                // Cache session
                _sessions[sessionId] = new ImportSession
                {
                    Rows = rows,
                    Columns = headers,
                    SalePlatformId = salePlatformId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                };

                // Get saved mapping if exists
                var savedMapping = await _importMappingPersistence.GetBySalePlatformIdAsync(salePlatformId);
                List<ImportColumnMappingDto>? savedMappingDto = null;
                if (savedMapping != null)
                {
                    savedMappingDto = JsonSerializer.Deserialize<List<ImportColumnMappingDto>>(savedMapping.MappingsJson);
                }

                return new ImportUploadResultDto
                {
                    SessionId = sessionId,
                    Columns = headers,
                    SampleRows = rows.Take(5).ToList(),
                    SavedMapping = savedMappingDto
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar CSV: {ex.Message}");
            }
        }

        public async Task<ImportPreviewDto> GeneratePreviewAsync(string sessionId, List<ImportColumnMappingDto> mappings, int salePlatformId)
        {
            try
            {
                var session = GetSession(sessionId);
                var mappingDict = mappings.ToDictionary(m => m.SeamstressField, m => m.CsvColumn);

                // Step 1: Group rows by ExternalId (Identificador URL) - one row per variant -> one product
                var groupedProducts = GroupAndNormalizeRows(session.Rows, mappingDict);

                // Step 2: Validate fabrics - must exist in DB
                var allFabrics = await _fabricPersistence.GetAllFabricsAsync();
                var validProducts = new List<NormalizedProduct>();
                var failedProducts = new List<ImportPreviewItemDto>();

                foreach (var product in groupedProducts)
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
                            FailReason = $"Tecido n\u00e3o encontrado: {product.Fabric}"
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

                // Step 3: Diff against existing items
                var existingItems = await _itemPersistence.GetItemsByExternalSourceAsync(salePlatformId);
                var existingDict = existingItems
                    .Where(i => i.ExternalId != null)
                    .ToDictionary(i => i.ExternalId!);

                var csvExternalIds = validProducts.Select(r => r.ExternalId).ToHashSet();

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

                // Classify: Inactivate (in DB but not in CSV)
                foreach (var existing in existingItems.Where(i => i.IsActive == true && i.ExternalId != null))
                {
                    if (!csvExternalIds.Contains(existing.ExternalId!))
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

                session.Preview = preview;
                session.MappingDict = mappingDict;
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
                    throw new Exception("Preview n\u00e3o gerado. Execute o preview antes de importar.");

                // Carry over failed imports from preview
                result.Failed = session.Preview.Failed;

                var preview = session.Preview;

                // Load all lookup data for entity resolution
                var allColors = (await _colorPersistence.GetAllColorsAsync()).ToList();
                var allFabrics = (await _fabricPersistence.GetAllFabricsAsync()).ToList();
                var allSizes = (await _sizePersistence.GetAllSizesAsync()).ToList();

                // Build normalized products from preview + session rows (for image URLs)
                var normalizedProducts = new Dictionary<string, NormalizedProduct>();
                foreach (var item in preview.ToCreate.Concat(preview.ToUpdate))
                {
                    normalizedProducts[item.ExternalId] = new NormalizedProduct
                    {
                        ExternalId = item.ExternalId,
                        Name = item.Name,
                        Price = item.Price,
                        Fabric = item.Fabric,
                        Colors = item.Colors,
                        Sizes = item.Sizes,
                        ImageUrls = FindImageUrlsForExternalId(session, item.ExternalId)
                    };
                }

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
                            .Where(c => colorMap.ContainsKey(c.ToLower()))
                            .Select(c => new ItemColor { ColorId = colorMap[c.ToLower()] }).ToList();
                        item.ItemFabrics = string.IsNullOrWhiteSpace(product.Fabric) ? new List<ItemFabric>()
                            : new List<ItemFabric> { new ItemFabric { FabricId = fabricMap[product.Fabric.ToLower()] } };
                        item.ItemSizes = product.Sizes
                            .Where(s => sizeMap.ContainsKey(s.ToLower()))
                            .Select(s => new ItemSize { SizeId = sizeMap[s.ToLower()] }).ToList();

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
                        var newColorIds = product.Colors.Where(c => colorMap.ContainsKey(c.ToLower())).Select(c => colorMap[c.ToLower()]).ToHashSet();

                        var colorsToRemove = tracked.ItemColors.Where(ic => !newColorIds.Contains(ic.ColorId)).ToArray();
                        if (colorsToRemove.Length > 0) _generalPersistence.DeleteRange(colorsToRemove);
                        foreach (var colorId in newColorIds.Where(id => !existingColorIds.Contains(id)))
                            _generalPersistence.Add(new ItemColor { ItemId = tracked.Id, ColorId = colorId });

                        // Update fabrics: remove old, add new
                        var existingFabricIds = tracked.ItemFabrics.Select(f => f.FabricId).ToHashSet();
                        var newFabricIds = string.IsNullOrWhiteSpace(product.Fabric) ? new HashSet<int>()
                            : new HashSet<int> { fabricMap[product.Fabric.ToLower()] };

                        var fabricsToRemove = tracked.ItemFabrics.Where(f => !newFabricIds.Contains(f.FabricId)).ToArray();
                        if (fabricsToRemove.Length > 0) _generalPersistence.DeleteRange(fabricsToRemove);
                        foreach (var fabricId in newFabricIds.Where(id => !existingFabricIds.Contains(id)))
                            _generalPersistence.Add(new ItemFabric { ItemId = tracked.Id, FabricId = fabricId });

                        // Update sizes: only add new, remove stale. NEVER recreate existing (preserves measurements)
                        var existingSizeIds = tracked.ItemSizes.Select(s => s.SizeId).ToHashSet();
                        var newSizeIds = product.Sizes.Where(s => sizeMap.ContainsKey(s.ToLower())).Select(s => sizeMap[s.ToLower()]).ToHashSet();

                        var sizesToRemove = tracked.ItemSizes.Where(s => !newSizeIds.Contains(s.SizeId)).ToArray();
                        if (sizesToRemove.Length > 0) _generalPersistence.DeleteRange(sizesToRemove);
                        foreach (var sizeId in newSizeIds.Where(id => !existingSizeIds.Contains(id)))
                            _generalPersistence.Add(new ItemSize { ItemId = tracked.Id, SizeId = sizeId });

                        _generalPersistence.Update(tracked);
                        result.Updated++;
                    }

                    await _generalPersistence.SaveChangesAsync();

                    // INACTIVATE items no longer in CSV
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

                // Clear change tracker to avoid conflicts when re-fetching items for image download
                _generalPersistence.ClearChangeTracker();

                // IMAGE DOWNLOAD (after transaction commits)
                await DownloadAndUploadImages(normalizedProducts, session.SalePlatformId, result);

                // Clean up session
                _sessions.TryRemove(sessionId, out _);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar importa\u00e7\u00e3o: {ex.Message}");
            }
        }

        public async Task<ImportMapping?> GetMappingAsync(int salePlatformId)
        {
            return await _importMappingPersistence.GetBySalePlatformIdAsync(salePlatformId);
        }

        public async Task<ImportMapping> SaveMappingAsync(int salePlatformId, List<ImportColumnMappingDto> mappings)
        {
            try
            {
                var existing = await _importMappingPersistence.GetBySalePlatformIdAsync(salePlatformId);
                var json = JsonSerializer.Serialize(mappings);

                if (existing != null)
                {
                    existing.MappingsJson = json;
                    _generalPersistence.Update(existing);
                }
                else
                {
                    var mapping = new ImportMapping
                    {
                        SalePlatformId = salePlatformId,
                        MappingsJson = json
                    };
                    _generalPersistence.Add(mapping);
                }

                await _generalPersistence.SaveChangesAsync();
                return (await _importMappingPersistence.GetBySalePlatformIdAsync(salePlatformId))!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar mapeamento: {ex.Message}");
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
                throw new Exception("Sess\u00e3o de importa\u00e7\u00e3o expirada ou n\u00e3o encontrada.");
            }
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(30); // refresh
            return session;
        }

        /// <summary>
        /// Groups CSV rows by ExternalId (one row per variant -> one product).
        /// Extracts colors/sizes from variation columns, fabric from first category.
        /// Filters out NAO visibility and E-book category.
        /// Extracts measurements description from Descricao field.
        /// </summary>
        private List<NormalizedProduct> GroupAndNormalizeRows(
            List<Dictionary<string, string>> rows,
            Dictionary<string, string> mappingDict)
        {
            // Known garment categories to ignore when looking for fabric
            var garmentCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Vestidos", "Blusas e Camisas", "Cal\u00e7as", "Conjuntos",
                "Kimonos", "Acess\u00f3rios", "E-book"
            };

            var visibilityCol = mappingDict.GetValueOrDefault("Visibility", "Exibir na loja");
            var categoriesCol = mappingDict.GetValueOrDefault("Categories", "Categorias");
            var externalIdCol = mappingDict.GetValueOrDefault("ExternalId", "Identificador URL");
            var nameCol = mappingDict.GetValueOrDefault("Name", "Nome");
            var priceCol = mappingDict.GetValueOrDefault("Price", "Pre\u00e7o");
            var descCol = mappingDict.GetValueOrDefault("Description", "Descri\u00e7\u00e3o");
            var var1NameCol = "Nome da varia\u00e7\u00e3o 1";
            var var1ValCol = "Valor da varia\u00e7\u00e3o 1";
            var var2NameCol = "Nome da varia\u00e7\u00e3o 2";
            var var2ValCol = "Valor da varia\u00e7\u00e3o 2";

            // Group rows by ExternalId
            var grouped = rows
                .Where(r => !string.IsNullOrWhiteSpace(GetValue(r, externalIdCol)))
                .GroupBy(r => GetValue(r, externalIdCol));

            var products = new List<NormalizedProduct>();

            foreach (var group in grouped)
            {
                var firstRow = group.First();

                // Filter: skip NAO visibility
                var visibility = GetValue(firstRow, visibilityCol);
                if (visibility.Contains("N", StringComparison.OrdinalIgnoreCase)) continue;
                if (visibility == "") continue; // only SIM rows have visibility set

                // Filter: skip E-book category
                var categories = GetValue(firstRow, categoriesCol);
                if (categories.Contains("E-book", StringComparison.OrdinalIgnoreCase)) continue;

                // Extract fabric: first category that is NOT a garment type
                var categoryList = categories.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();
                var fabric = categoryList.FirstOrDefault(c => !garmentCategories.Contains(c)) ?? "";

                // Collect colors and sizes from ALL variant rows
                var colors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var sizes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var row in group)
                {
                    ExtractVariant(row, var1NameCol, var1ValCol, colors, sizes);
                    ExtractVariant(row, var2NameCol, var2ValCol, colors, sizes);
                }

                // Extract measurements from description
                var description = GetValue(firstRow, descCol);
                var measurements = ExtractMeasurements(description);

                // Extract image URLs if mapped
                var imageUrls = new List<string>();
                // Images are typically not in the NuvemShop CSV export - handled separately

                products.Add(new NormalizedProduct
                {
                    ExternalId = group.Key,
                    Name = GetValue(firstRow, nameCol),
                    Price = ParseDecimal(GetValue(firstRow, priceCol)),
                    Fabric = fabric,
                    Colors = colors.ToList(),
                    Sizes = sizes.ToList(),
                    ImageUrls = imageUrls,
                    MeasurementsDescription = measurements
                });
            }

            return products;
        }

        private void ExtractVariant(Dictionary<string, string> row, string nameCol, string valCol,
            HashSet<string> colors, HashSet<string> sizes)
        {
            var varName = GetValue(row, nameCol).Trim();
            var varValue = GetValue(row, valCol).Trim();
            if (string.IsNullOrEmpty(varName) || string.IsNullOrEmpty(varValue)) return;

            if (varName.Contains("Cor", StringComparison.OrdinalIgnoreCase))
                colors.Add(varValue);
            else if (varName.Contains("Tamanho", StringComparison.OrdinalIgnoreCase))
                sizes.Add(varValue);
        }

        private string ExtractMeasurements(string htmlDescription)
        {
            if (string.IsNullOrWhiteSpace(htmlDescription)) return "";

            // Strip HTML tags
            var text = System.Text.RegularExpressions.Regex.Replace(htmlDescription, @"<[^>]+>", " ");
            // Decode HTML entities
            text = System.Net.WebUtility.HtmlDecode(text);
            // Normalize whitespace
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();

            // Find MEDIDA/Medidas section
            var idx = text.IndexOf("MEDIDA", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";

            return text.Substring(idx).Trim();
        }

        private string GetValue(Dictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out var val) ? val?.Trim() ?? "" : "";
        }

        private decimal ParseDecimal(string value)
        {
            var cleaned = value.Replace("R$", "").Replace(" ", "").Trim();
            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            if (decimal.TryParse(cleaned, NumberStyles.Any, new CultureInfo("pt-BR"), out result))
                return result;
            return 0;
        }

        private List<string> DetectChanges(Item existing, NormalizedProduct incoming)
        {
            var changes = new List<string>();

            if (existing.Name != incoming.Name)
                changes.Add($"Nome: {existing.Name} \u2192 {incoming.Name}");
            if (existing.Price != incoming.Price)
                changes.Add($"Pre\u00e7o: {existing.Price} \u2192 {incoming.Price}");

            var existingColors = existing.ItemColors.Select(ic => ic.Color?.Name?.ToLower()).OrderBy(c => c).ToList();
            var incomingColors = incoming.Colors.Select(c => c.ToLower()).OrderBy(c => c).ToList();
            if (!existingColors.SequenceEqual(incomingColors))
                changes.Add($"Cores: [{string.Join(", ", existingColors)}] \u2192 [{string.Join(", ", incomingColors)}]");

            var existingFabric = existing.ItemFabrics.FirstOrDefault()?.Fabric?.Name?.ToLower() ?? "";
            if (existingFabric != incoming.Fabric.ToLower())
                changes.Add($"Tecido: {existingFabric} \u2192 {incoming.Fabric}");

            var existingSizes = existing.ItemSizes.Select(isz => isz.Size?.Name?.ToLower()).OrderBy(s => s).ToList();
            var incomingSizes = incoming.Sizes.Select(s => s.ToLower()).OrderBy(s => s).ToList();
            if (!existingSizes.SequenceEqual(incomingSizes))
                changes.Add($"Tamanhos: [{string.Join(", ", existingSizes)}] \u2192 [{string.Join(", ", incomingSizes)}]");

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

            foreach (var name in names)
            {
                // Use reflection to get Name, Id, IsActive (all lookup entities share this shape)
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
                foreach (var name in names)
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

        private List<string> FindImageUrlsForExternalId(ImportSession session, string externalId)
        {
            if (session.MappingDict == null) return new List<string>();
            if (!session.MappingDict.TryGetValue("ImageURL", out var imageColumn)) return new List<string>();
            if (!session.MappingDict.TryGetValue("ExternalId", out var idColumn)) return new List<string>();

            var row = session.Rows.LastOrDefault(r =>
                r.TryGetValue(idColumn, out var id) && id?.Trim() == externalId);
            if (row == null || !row.TryGetValue(imageColumn, out var urls)) return new List<string>();

            return SplitValues(urls);
        }

        private List<string> SplitValues(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new List<string>();
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();
        }

        private async Task DownloadAndUploadImages(
            Dictionary<string, NormalizedProduct> products,
            int salePlatformId,
            ImportResultDto result)
        {
            // Re-fetch items (tracked) to update ImageURL
            var allItems = await _itemPersistence.GetItemsByExternalSourceAsync(salePlatformId);

            foreach (var item in allItems.Where(i => i.IsActive == true && i.ExternalId != null))
            {
                if (!products.TryGetValue(item.ExternalId!, out var product)) continue;
                if (product.ImageUrls.Count == 0) continue;

                // Compare existing image URLs with new ones to avoid re-downloading
                var existingBlobs = string.IsNullOrEmpty(item.ImageURL)
                    ? new List<string>()
                    : item.ImageURL.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(b => b.Trim()).ToList();

                // For new items or items with changed images, download all
                // (We can't compare CDN URLs to blob names, so on update we re-download)
                var blobNames = new List<string>();
                foreach (var imageUrl in product.ImageUrls)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(imageUrl)) continue;
                        var url = imageUrl.StartsWith("http") ? imageUrl : $"https:{imageUrl}";

                        using var response = await _httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        using var stream = await response.Content.ReadAsStreamAsync();

                        var extension = Path.GetExtension(new Uri(url).AbsolutePath);
                        if (string.IsNullOrEmpty(extension)) extension = ".jpg";
                        var imageName = $"{Guid.NewGuid()}{extension}";

                        var blobName = await _azureBlobService.UploadModelImageAsync(stream, imageName);
                        blobNames.Add(blobName);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Erro ao baixar imagem para {item.Name}: {ex.Message}");
                    }
                }

                if (blobNames.Count > 0)
                {
                    // Delete old blobs (orphaned)
                    foreach (var oldBlob in existingBlobs)
                    {
                        _azureBlobService.DeleteModelImage(oldBlob);
                    }

                    item.ImageURL = string.Join(";", blobNames);
                    _generalPersistence.Update(item);
                }
            }

            await _generalPersistence.SaveChangesAsync();
        }

        #endregion

        #region Private Classes

        private class ImportSession
        {
            public List<Dictionary<string, string>> Rows { get; set; } = new();
            public List<string> Columns { get; set; } = new();
            public int SalePlatformId { get; set; }
            public ImportPreviewDto? Preview { get; set; }
            public Dictionary<string, string>? MappingDict { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        private class NormalizedProduct
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

        #endregion
    }
}
