using Microsoft.EntityFrameworkCore;

namespace Seamstress.Persistence.Helpers
{
  public class PageList<T> : List<T>
  {
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PageList() { }

    public PageList(List<T> items, int count, int pageNumber, int pageSize)
    {
      TotalCount = count;
      CurrentPage = pageNumber;
      PageSize = pageSize;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);
      AddRange(items);
    }

    public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
      int count = await source.CountAsync();
      List<T> items = await source.Skip((pageNumber - 1) * pageSize) // Basicamente faz a função de pular os itens das paginas anteriores (-1 para acessar o indice correto da pagina)
                              .Take(pageSize).ToListAsync(); // Pega a quantidade de itens desejados e retorna como uma lista

      return new PageList<T>(items, count, pageNumber, pageSize);
    }
  }
}