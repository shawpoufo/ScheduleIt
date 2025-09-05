using System;
using MediatR;

namespace Application.Customers.Queries.GetCustomer
{
    public sealed record GetCustomerQuery(Guid Id) : IRequest<CustomerDto>;

    public sealed record CustomerDto(Guid Id, string Name, string Email);
}
