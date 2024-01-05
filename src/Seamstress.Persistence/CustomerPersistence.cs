using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Helpers;


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
      IQueryable<Customer> query = _context.Customers;

      query = query.Include(customer => customer.Sizings);
      query = query.Where(customer => customer.Name.ToLower().Contains(pageParams.Term.ToLower()))
                   .OrderBy(customer => customer.Name.Trim().ToLower()).AsNoTracking();

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

      return await query.AsNoTracking().FirstAsync();
    }
  }
}