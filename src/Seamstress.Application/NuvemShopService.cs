using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;

namespace Seamstress.Application
{
    public class NuvemShopService : INuvemShopService
    {
        private readonly HttpClient _httpClient;
        private readonly IImportService _importService;
        private const int SalePlatformId = 1; // "Ecommerce" in SalePlatforms table

        public NuvemShopService(
            HttpClient httpClient,
            IImportService importService)
        {
            _httpClient = httpClient;
            _importService = importService;

            var accessToken = Environment.GetEnvironmentVariable("NUVEMSHOP_ACCESS_TOKEN")!;
            var userAgent = Environment.GetEnvironmentVariable("NUVEMSHOP_USER_AGENT")!;
            var storeId = Environment.GetEnvironmentVariable("NUVEMSHOP_STORE_ID")!;

            _httpClient.BaseAddress = new Uri($"https://api.nuvemshop.com.br/v1/{storeId}/");
            _httpClient.DefaultRequestHeaders.Add("Authentication", $"bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
        }

        public async Task<ImportPreviewDto> FetchAndPreviewAsync()
        {
            var products = await FetchAllProductsAsync();

            if (products.Count == 0)
                return new ImportPreviewDto
                {
                    SessionId = "",
                    ToCreate = new List<ImportPreviewItemDto>(),
                    ToUpdate = new List<ImportPreviewItemDto>(),
                    ToInactivate = new List<ImportPreviewItemDto>(),
                    Failed = new List<ImportPreviewItemDto>()
                };

            var (normalized, failed) = NormalizeProducts(products);
            var preview = await _importService.GeneratePreviewAsync(normalized, SalePlatformId);
            preview.Failed.AddRange(failed);
            return preview;
        }

        private async Task<List<JsonElement>> FetchAllProductsAsync()
        {
            var allProducts = new List<JsonElement>();
            int page = 1;
            const int perPage = 200;

            while (true)
            {
                var response = await _httpClient.GetAsync($"products?published=true&per_page={perPage}&page={page}");

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar produtos da NuvemShop (HTTP {(int)response.StatusCode}): {body}");
                }

                var json = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
                if (json == null || json.Count == 0) break;

                allProducts.AddRange(json);
                if (json.Count < perPage) break;
                page++;
            }

            return allProducts;
        }

        private (List<NormalizedProduct> normalized, List<ImportPreviewItemDto> failed) NormalizeProducts(List<JsonElement> products)
        {
            var normalized = new List<NormalizedProduct>();
            var failed = new List<ImportPreviewItemDto>();

            foreach (var product in products)
            {
                var productName = product.TryGetProperty("name", out var nameObj) && nameObj.TryGetProperty("pt", out var namePt)
                    ? namePt.GetString() ?? "" : "";
                var productId = product.TryGetProperty("id", out var idEl) ? idEl.GetInt64().ToString() : "";

                // Fail: no categories
                if (!product.TryGetProperty("categories", out var categories) || categories.GetArrayLength() == 0)
                {
                    failed.Add(new ImportPreviewItemDto { ExternalId = productId, Name = productName, Action = "Failed", FailReason = "Produto sem categorias (tecido)" });
                    continue;
                }

                // Fail: no variants
                if (!product.TryGetProperty("variants", out var variants) || variants.GetArrayLength() == 0)
                {
                    failed.Add(new ImportPreviewItemDto { ExternalId = productId, Name = productName, Action = "Failed", FailReason = "Produto sem variantes (cores/tamanhos)" });
                    continue;
                }

                // Get attribute indices for Cor/Tamanho
                var (colorIndex, sizeIndex) = GetAttributeIndices(product);

                // Extract colors and sizes from variants
                var colors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var sizes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var variant in variants.EnumerateArray())
                {
                    if (variant.TryGetProperty("values", out var values))
                    {
                        var valuesArr = values.EnumerateArray().ToList();
                        if (colorIndex >= 0 && colorIndex < valuesArr.Count)
                        {
                            var color = valuesArr[colorIndex].GetProperty("pt").GetString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(color)) colors.Add(color);
                        }
                        if (sizeIndex >= 0 && sizeIndex < valuesArr.Count)
                        {
                            var size = valuesArr[sizeIndex].GetProperty("pt").GetString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(size)) sizes.Add(size);
                        }
                    }
                }

                // Parse price from first variant
                var firstVariant = variants.EnumerateArray().First();
                var priceStr = firstVariant.GetProperty("price").GetString() ?? "0";
                decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var price);

                // Extract image URLs
                var imageUrls = new List<string>();
                if (product.TryGetProperty("images", out var images))
                {
                    foreach (var img in images.EnumerateArray())
                    {
                        var src = img.GetProperty("src").GetString();
                        if (!string.IsNullOrEmpty(src)) imageUrls.Add(src);
                    }
                }

                // Parse Medidas from description
                var description = "";
                if (product.TryGetProperty("description", out var desc) &&
                    desc.TryGetProperty("pt", out var descPt))
                {
                    description = ParseMedidas(descPt.GetString() ?? "");
                }

                // Fabric = first category name
                var fabric = (categories.EnumerateArray().First()
                    .GetProperty("name").GetProperty("pt").GetString() ?? "").Trim();

                normalized.Add(new NormalizedProduct
                {
                    ExternalId = productId,
                    Name = productName,
                    Price = price,
                    Fabric = fabric,
                    Colors = colors.ToList(),
                    Sizes = sizes.ToList(),
                    ImageUrls = imageUrls,
                    MeasurementsDescription = description
                });
            }

            return (normalized, failed);
        }

        private (int colorIndex, int sizeIndex) GetAttributeIndices(JsonElement product)
        {
            int colorIndex = -1;
            int sizeIndex = -1;

            if (product.TryGetProperty("attributes", out var attributes))
            {
                int i = 0;
                foreach (var attr in attributes.EnumerateArray())
                {
                    var name = attr.GetProperty("pt").GetString()?.ToLower() ?? "";
                    if (name.StartsWith("cor")) colorIndex = i;
                    else if (name.StartsWith("tamanho")) sizeIndex = i;
                    i++;
                }
            }

            return (colorIndex, sizeIndex);
        }

        private static string ParseMedidas(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";

            // Find "Medidas" keyword (case-insensitive)
            var medidasMatch = Regex.Match(html, @"medidas", RegexOptions.IgnoreCase);
            if (!medidasMatch.Success) return "";

            // Extract from Medidas to end or dash delimiter
            var fromMedidas = html.Substring(medidasMatch.Index);
            var dashMatch = Regex.Match(fromMedidas, @"-{3,}");
            if (dashMatch.Success)
                fromMedidas = fromMedidas.Substring(0, dashMatch.Index);

            // Convert block-level tags to newlines before stripping
            var text = Regex.Replace(fromMedidas, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</p>", "\n", RegexOptions.IgnoreCase);

            // Strip remaining HTML tags
            text = Regex.Replace(text, @"<[^>]+>", "");

            // Decode HTML entities
            text = System.Net.WebUtility.HtmlDecode(text);

            // Clean up whitespace: replace multiple spaces/tabs with single space, keep newlines
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n\s*\n", "\n");
            text = text.Trim();

            return text;
        }
    }
}
