using MediatR;

namespace Application.Customers.Commands.CreateCustomer
{
    public sealed record CreateCustomerCommand(string Name, string Email) : IRequest<System.Guid>;
}


