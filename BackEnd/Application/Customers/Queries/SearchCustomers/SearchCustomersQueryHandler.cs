using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using MediatR;

namespace Application.Customers.Queries.SearchCustomers
{
    public sealed class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, IEnumerable<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        public SearchCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.SearchAsync(request.Search, cancellationToken);

            return customers.Select(c => new CustomerDto(c.Id, c.Name, c.Email));
        }
    }
}
