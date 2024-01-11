using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Helpers;
using System.Text;
using System.Linq.Expressions;
using static Seamstress.Persistence.Helpers.CombineEpressions;


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
      List<string> terms = normalizedTerm.Split(" ").ToList();

      IQueryable<Customer> query = _context.Customers;
      query = query.Include(customer => customer.Sizings);

      if (terms.Count > 0)
      {
        Expression<Func<Customer, bool>> expression = customer =>
            EF.Functions.Unaccent(customer.Name.ToLower()).StartsWith(terms[0].ToLower());

        terms.ForEach(term =>
        {
          Expression<Func<Customer, bool>> termExpression = customer =>
              EF.Functions.Unaccent(customer.Name.ToLower()).Contains(term.ToLower());

          expression = CombineExpressions(expression, termExpression);

        });

        query = query.Where(expression);
      }

      query = query.OrderBy(customer => customer.Name.Trim().ToLower()).AsNoTracking();

      return await PageList<Customer>.CreateAsync(query, pageParams.PageNumber, pageParams.PageSize);
    }

    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Include(customer => customer.Sizings);
      query = query.Where(customer => customer.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }

    public async Task<Customer> GetCustomerByPKAsync(string CPF_CNPJ)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Where(customer => customer.CPF_CNPJ == CPF_CNPJ);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }
  }
}