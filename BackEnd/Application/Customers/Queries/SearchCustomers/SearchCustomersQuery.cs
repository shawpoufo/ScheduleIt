using System.Collections.Generic;
using MediatR;

namespace Application.Customers.Queries.SearchCustomers
{
    public sealed record SearchCustomersQuery(string? Search) : IRequest<IEnumerable<CustomerDto>>;
}
