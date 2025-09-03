using System;
using Domain.Common;
using MediatR;

namespace Domain.Events
{
    public sealed class AppointmentBooked : IDomainEvent, INotification
    {
        public Guid CustomerId { get; }
        public DateTime StartUtc { get; }
        public DateTime EndUtc { get; }

        public AppointmentBooked(Guid customerId, DateTime startUtc, DateTime endUtc)
        {
            CustomerId = customerId;
            StartUtc = startUtc;
            EndUtc = endUtc;
        }
    }
}


