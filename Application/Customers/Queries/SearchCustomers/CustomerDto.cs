using System;

namespace Application.Customers.Queries.SearchCustomers
{
    public sealed record CustomerDto(Guid Id, string Name, string Email);
}
