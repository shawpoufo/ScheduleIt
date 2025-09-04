using System;
using Domain.Common;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    // Domain/Entities/Appointment.cs
    public sealed class Appointment : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public AppointmentTimeSlot TimeSlot { get; private set; }
        public AppointmentStatus Status { get; private set; }

        // Parameterless constructor for EF Core
        private Appointment() : base(Guid.NewGuid())
        {
            Status = AppointmentStatus.Scheduled;
        }

        private Appointment(Guid customerId, AppointmentTimeSlot timeSlot) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            TimeSlot = timeSlot;
            Status = AppointmentStatus.Scheduled;
        }

        public static Appointment Create(Guid customerId, DateTime startUtc, DateTime now)
        {
            var timeSlot = AppointmentTimeSlot.Create(startUtc, now);

            var appointment = new Appointment(customerId, timeSlot);
            appointment.AddEvent(new AppointmentBooked(customerId, timeSlot));
            return appointment;
        }

        public void Cancel(DateTime now)
        {
            if (Status == AppointmentStatus.Canceled)
                throw new InvalidOperationException("Appointment is already canceled.");

            // Domain invariant: Prevent cancel after start
            if (TimeSlot.StartUtc <= now)
                throw new InvalidOperationException("Cannot cancel an appointment that has already started or finished.");

            Status = AppointmentStatus.Canceled;
            AddEvent(new AppointmentCanceled(Id));
        }

        public void MarkAsCompleted()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new InvalidOperationException("Cannot mark a canceled appointment as completed.");

            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Appointment is already marked as completed.");

            Status = AppointmentStatus.Completed;
        }

        public void MarkAsInProgress()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new InvalidOperationException("Cannot mark a canceled appointment as in progress.");

            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot mark a completed appointment as in progress.");

            if (Status == AppointmentStatus.InProgress)
                throw new InvalidOperationException("Appointment is already in progress.");

            Status = AppointmentStatus.InProgress;
        }

        public void MarkAsNoShow()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new InvalidOperationException("Cannot mark a canceled appointment as no-show.");

            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot mark a completed appointment as no-show.");

            if (Status == AppointmentStatus.NoShow)
                throw new InvalidOperationException("Appointment is already marked as no-show.");

            Status = AppointmentStatus.NoShow;
        }

        public bool IsOverlapping(DateTime startTime, DateTime endTime)
        {
            return Status != AppointmentStatus.Canceled && TimeSlot.IsOverlapping(startTime, endTime);
        }

        public bool IsActive => Status == AppointmentStatus.Scheduled || Status == AppointmentStatus.InProgress;
    }
}
