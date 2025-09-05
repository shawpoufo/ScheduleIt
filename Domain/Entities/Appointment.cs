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
        public string Notes { get; private set; } = string.Empty;

        // Parameterless constructor for EF Core
        private Appointment() : base(Guid.NewGuid())
        {
            Status = AppointmentStatus.Scheduled;
        }

        private Appointment(Guid customerId, AppointmentTimeSlot timeSlot, string notes) : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            TimeSlot = timeSlot;
            Status = AppointmentStatus.Scheduled;
            Notes = notes ?? string.Empty;
        }

        public static Appointment Create(Guid customerId, DateTime startUtc, DateTime endUtc, DateTime now, string? notes = null)
        {
            var timeSlot = AppointmentTimeSlot.Create(startUtc, endUtc, now);

            var appointment = new Appointment(customerId, timeSlot, notes ?? string.Empty);
            appointment.AddEvent(new AppointmentBooked(customerId, timeSlot));
            return appointment;
        }

        public void Cancel(DateTime now)
        {
            if (Status == AppointmentStatus.Canceled)
                throw new DomainRuleViolationException("Appointment is already canceled.");

            // Domain invariant: Prevent cancel after start
            if (TimeSlot.StartUtc <= now)
                throw new DomainRuleViolationException("Cannot cancel an appointment that has already started or finished.");

            Status = AppointmentStatus.Canceled;
            AddEvent(new AppointmentCanceled(Id));
        }

        public void MarkAsCompleted()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new DomainRuleViolationException("Cannot mark a canceled appointment as completed.");

            if (Status == AppointmentStatus.Completed)
                throw new DomainRuleViolationException("Appointment is already marked as completed.");

            Status = AppointmentStatus.Completed;
        }

        public void MarkAsInProgress()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new DomainRuleViolationException("Cannot mark a canceled appointment as in progress.");

            if (Status == AppointmentStatus.Completed)
                throw new DomainRuleViolationException("Cannot mark a completed appointment as in progress.");

            if (Status == AppointmentStatus.InProgress)
                throw new DomainRuleViolationException("Appointment is already in progress.");

            Status = AppointmentStatus.InProgress;
        }

        public void MarkAsNoShow()
        {
            if (Status == AppointmentStatus.Canceled)
                throw new DomainRuleViolationException("Cannot mark a canceled appointment as no-show.");

            if (Status == AppointmentStatus.Completed)
                throw new DomainRuleViolationException("Cannot mark a completed appointment as no-show.");

            if (Status == AppointmentStatus.NoShow)
                throw new DomainRuleViolationException("Appointment is already marked as no-show.");

            Status = AppointmentStatus.NoShow;
        }

        public void MarkAsScheduled()
        {
            // Idempotent if already scheduled; otherwise disallow reverting
            if (Status == AppointmentStatus.Scheduled)
                return;

            throw new DomainRuleViolationException("Cannot revert an appointment back to Scheduled.");
        }

        public bool IsOverlapping(DateTime startTime, DateTime endTime)
        {
            return Status != AppointmentStatus.Canceled && TimeSlot.IsOverlapping(startTime, endTime);
        }

        public bool IsActive => Status == AppointmentStatus.Scheduled || Status == AppointmentStatus.InProgress;
    }
}
