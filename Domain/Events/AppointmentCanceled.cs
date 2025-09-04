using System;
using Domain.Common;
using MediatR;

namespace Domain.Events
{
    public sealed class AppointmentCanceled : IDomainEvent, INotification
    {
        public Guid AppointmentId { get; }

        public AppointmentCanceled(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}

