using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        void Add(Customer customer);
    }
}


