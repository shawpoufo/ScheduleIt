using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Customer : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        private Customer() { } // For EF Core

        public Customer(Guid id, string name, string email) : base(id)
        {
            Name = name;
            Email = email;
        }
    }
}
