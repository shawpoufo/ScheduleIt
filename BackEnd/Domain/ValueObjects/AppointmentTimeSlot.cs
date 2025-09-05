using System;
using Domain.Common;

namespace Domain.ValueObjects
{
    public sealed record AppointmentTimeSlot
    {
        public DateTime StartUtc { get; }
        public DateTime EndUtc { get; }

        private AppointmentTimeSlot(DateTime startUtc, DateTime endUtc)
        {
            StartUtc = startUtc;
            EndUtc = endUtc;
        }

        public static AppointmentTimeSlot Create(DateTime startUtc, DateTime endUtc, DateTime now)
        {
            if (startUtc <= now)
                throw new DomainRuleViolationException("Appointment cannot be scheduled in the past.");

            if (startUtc >= endUtc)
                throw new DomainRuleViolationException("Start time must be before end time.");

            // Reasonable duration guardrails (e.g., > 30 minutes and < 12 hours)
            var duration = endUtc - startUtc;
            if (duration.TotalMinutes < 30)
                throw new DomainRuleViolationException("Appointment duration must be at least 30 minutes.");
            if (duration.TotalHours > 12)
                throw new DomainRuleViolationException("Appointment duration cannot exceed 12 hours.");

            return new AppointmentTimeSlot(startUtc, endUtc);
        }

        public static AppointmentTimeSlot CreateFromStartAndEnd(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc >= endUtc)
                throw new ArgumentException("Start time must be before end time.");

            return new AppointmentTimeSlot(startUtc, endUtc);
        }

        public bool IsOverlapping(AppointmentTimeSlot other)
        {
            return StartUtc < other.EndUtc && EndUtc > other.StartUtc;
        }

        public bool IsOverlapping(DateTime startTime, DateTime endTime)
        {
            return StartUtc < endTime && EndUtc > startTime;
        }
    }
}
