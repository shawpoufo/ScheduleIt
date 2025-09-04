using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using MediatR;
using Application.Common;

namespace Application.Customers.Queries.GetCustomer
{
    public sealed class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID '{request.Id}' not found.");
            }

            return new CustomerDto(customer.Id, customer.Name, customer.Email);
        }
    }
}
