using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        protected Entity(Guid id)
        {
            Id = id;
        }

    }
}
