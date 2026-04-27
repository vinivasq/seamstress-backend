using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Dtos;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Helpers;
using System.Text;
using System.Linq.Expressions;
using static Seamstress.Persistence.Helpers.CombineEpressions;
using Seamstress.Persistence.Parameters;


namespace Seamstress.Persistence
{
  public class CustomerPersistence : ICustomerPersistence
  {
    private readonly SeamstressContext _context;

    public CustomerPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<PageList<Customer>> GetCustomersAsync(PageParams pageParams)
    {
      string normalizedTerm = new(pageParams.Term.Normalize(NormalizationForm.FormD).ToArray());
      List<string> terms = normalizedTerm.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
      string digitsOnlyTerm = new(pageParams.Term.Where(char.IsDigit).ToArray());

      IQueryable<Customer> query = _context.Customers;
      query = query.Include(customer => customer.Sizings);

      if (terms.Count > 0)
      {
        Expression<Func<Customer, bool>> expression = customer =>
            EF.Functions.Unaccent(customer.Name.ToLower()).Contains(terms[0].ToLower());

        terms.Skip(1).ToList().ForEach(term =>
        {
          Expression<Func<Customer, bool>> termExpression = customer =>
              EF.Functions.Unaccent(customer.Name.ToLower()).Contains(term.ToLower());

          expression = CombineExpressions(expression, termExpression);
        });

        query = query.Where(expression);

        if (digitsOnlyTerm.Length > 0)
        {
          query = query.Union(_context.Customers
            .Where(x => x.CPF_CNPJ.Contains(digitsOnlyTerm))
            .Include(x => x.Sizings));
        }
      }

      query = query.OrderBy(customer => customer.Name.Trim().ToLower()).AsNoTracking();

      return await PageList<Customer>.CreateAsync(query, pageParams.PageNumber, pageParams.PageSize);
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Include(customer => customer.Sizings);
      query = query.Where(customer => customer.Id == id);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<Customer?> GetCustomerByPKAsync(string CPF_CNPJ)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Where(customer => customer.CPF_CNPJ == CPF_CNPJ);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<bool> CheckFKAsync(int id)
    {
      return await this._context.Orders.AnyAsync(x => x.CustomerId == id);
    }

    public async Task<List<CustomerExportDto>> GetCustomersForExportAsync(CustomerExportParams exportParams)
    {
      IQueryable<Customer> query = _context.Customers;

      if (exportParams.FromCreatedAt.HasValue)
        query = query.Where(c => c.CreatedAt >= exportParams.FromCreatedAt.Value);
      if (exportParams.ToCreatedAt.HasValue)
        query = query.Where(c => c.CreatedAt <= exportParams.ToCreatedAt.Value);

      if (exportParams.FromOrderDate.HasValue || exportParams.ToOrderDate.HasValue)
      {
        IQueryable<Order> orderQuery = _context.Orders;
        if (exportParams.FromOrderDate.HasValue)
          orderQuery = orderQuery.Where(o => o.OrderedAt >= exportParams.FromOrderDate.Value);
        if (exportParams.ToOrderDate.HasValue)
          orderQuery = orderQuery.Where(o => o.OrderedAt <= exportParams.ToOrderDate.Value);

        var customerIds = orderQuery.Select(o => o.CustomerId).Distinct();
        query = query.Where(c => customerIds.Contains(c.Id));
      }

      return await query
        .Select(c => new CustomerExportDto
        {
          Name = c.Name,
          Email = c.Email,
          PhoneNumber = c.PhoneNumber,
          Cep = c.Cep,
          City = c.City,
          UF = c.UF,
          OrdersTotal = _context.Orders
            .Where(o => o.CustomerId == c.Id)
            .Sum(o => (decimal?)o.Total) ?? 0
        })
        .AsNoTracking()
        .ToListAsync();
    }
  }
}
