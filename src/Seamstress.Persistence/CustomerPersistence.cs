using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;


namespace Seamstress.Persistence
{
  public class CustomerPersistence : ICustomerPersistence
  {
    private readonly SeamstressContext _context;

    public CustomerPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<Customer[]> GetCustomersAsync(string term)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Include(customer => customer.Sizings);
      query = query.Where(customer => customer.Name.ToLower().Contains(term.ToLower()))
                   .OrderBy(customer => customer.Name.Trim().ToLower());

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
      IQueryable<Customer> query = _context.Customers;

      query = query.Include(customer => customer.Sizings);
      query = query.Where(customer => customer.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}