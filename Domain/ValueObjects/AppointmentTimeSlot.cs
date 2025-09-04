using System;

namespace Domain.ValueObjects
{
    public sealed record AppointmentTimeSlot
    {
        private const int DurationMinutes = 30;
        
        public DateTime StartUtc { get; }
        public DateTime EndUtc { get; }

        private AppointmentTimeSlot(DateTime startUtc, DateTime endUtc)
        {
            StartUtc = startUtc;
            EndUtc = endUtc;
        }

        public static AppointmentTimeSlot Create(DateTime startUtc, DateTime now)
        {
            if (startUtc <= now)
                throw new ArgumentException("Appointment cannot be scheduled in the past.", nameof(startUtc));

            var endUtc = startUtc.AddMinutes(DurationMinutes);
            return new AppointmentTimeSlot(startUtc, endUtc);
        }

        public static AppointmentTimeSlot CreateFromStartAndEnd(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc >= endUtc)
                throw new ArgumentException("Start time must be before end time.", nameof(startUtc));

            var duration = endUtc - startUtc;
            if (duration.TotalMinutes != DurationMinutes)
                throw new ArgumentException($"Appointment duration must be exactly {DurationMinutes} minutes.", nameof(endUtc));

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
