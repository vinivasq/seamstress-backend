using System.Text.Json;
using Seamstress.API.Models;

namespace Seamstress.API.Extensions
{
  public static class Pagination
  {
    public static void AddPagination(this HttpResponse response, int currentPage, int pageSize, int totalItems, int totalPages)
    {
      response.Headers.Add("Pagination", JsonSerializer.Serialize(new PaginationHeader(currentPage, pageSize, totalItems, totalPages) { },
                                                                  new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

      response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // Necessário para expor o header de pagination
    }
  }
}