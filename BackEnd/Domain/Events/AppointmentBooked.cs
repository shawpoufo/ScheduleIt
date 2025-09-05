using System;
using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    public sealed class AppointmentBooked : IDomainEvent
    {
        public Guid CustomerId { get; }
        public AppointmentTimeSlot TimeSlot { get; }

        public AppointmentBooked(Guid customerId, AppointmentTimeSlot timeSlot)
        {
            CustomerId = customerId;
            TimeSlot = timeSlot;
        }
    }
}

