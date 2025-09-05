using System;
using Domain.Entities;
using Domain.Repositories;
using Application.Abstractions;
using MediatR;
using Application.Common;

namespace Application.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Check if customer with same email already exists
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingCustomer != null)
            {
                throw new ValidationException($"Customer with email '{request.Email}' already exists.");
            }

            var customer = new Customer(Guid.NewGuid(), request.Name, request.Email);

            _customerRepository.Add(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }
    }
}
