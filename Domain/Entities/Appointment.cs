using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Events;

namespace Domain.Entities
{
    // Domain/Entities/Appointment.cs
    public sealed class Appointment : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public DateTime StartUtc { get; private set; }
        public DateTime EndUtc { get; private set; } // Start + 30m

        private Appointment(Guid customerId , DateTime startUtc) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            StartUtc = startUtc;
        }

        public static Appointment Create(Guid customerId, DateTime startUtc)
        {
            var end = startUtc.AddMinutes(30);
            var app = new Appointment(customerId, startUtc);
            app.EndUtc = end;
            app.AddEvent(new AppointmentBooked(customerId, startUtc, end));
            return app;
        }

        public void Cancel() => AddEvent(new AppointmentCanceled(Id));
    }

}
