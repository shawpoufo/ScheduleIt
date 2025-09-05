using Domain.Events;
using MediatR;

namespace Application.DomainEvents
{
    public class AppointmentCanceledEventHandler : INotificationHandler<AppointmentCanceled>
    {
        public async Task Handle(AppointmentCanceled notification, CancellationToken cancellationToken)
        {
            // Handle appointment canceled event
            // e.g., send cancellation emails, update calendars, etc.
            var appointmentId = notification.AppointmentId;

            await Task.CompletedTask;
        }
    }
}


