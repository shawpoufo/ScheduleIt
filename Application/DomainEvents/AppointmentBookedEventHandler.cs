using Domain.Common;
using Domain.Events;
using MediatR;

namespace Application.DomainEvents
{
    public class AppointmentBookedEventHandler : INotificationHandler<AppointmentBooked>
    {
        public async Task Handle(AppointmentBooked notification, CancellationToken cancellationToken)
        {
            // Handle appointment booked event
            // e.g., send confirmation emails, update calendars, etc.
            var customerId = notification.CustomerId;
            var timeSlot = notification.TimeSlot;

            await Task.CompletedTask;
        }
    }
}


