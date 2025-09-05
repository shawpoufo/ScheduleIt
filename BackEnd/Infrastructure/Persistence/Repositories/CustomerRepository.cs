using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = _context.Customers.AsQueryable();

            // If search term is provided, filter by name (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(normalizedSearchTerm) || c.Email.ToLower().Contains(normalizedSearchTerm));
            }

            return await query
                .OrderBy(c => c.Name)
                .Take(20) // Limit results for performance
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids.Distinct().ToList();
            if (idList.Count == 0)
                return new List<Customer>();

            return await _context.Customers
                .Where(c => idList.Contains(c.Id))
                .ToListAsync(cancellationToken);
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }
    }
}


