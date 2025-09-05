using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Appointments.Queries.GetTodayStats
{
    public sealed record GetTodayStatsQuery : IRequest<TodayStatsDto>
    {
        public DateTime TodayUtc { get; }

        public GetTodayStatsQuery(DateTime? todayUtc = null)
        {
            TodayUtc = todayUtc ?? DateTime.UtcNow;
        }
    }

    public sealed record TodayStatsDto(
        int TotalAppointments,
        int TodayAppointments,
        IReadOnlyList<TodayUpcomingAppointmentDto> UpcomingToday
    );

    public sealed record TodayUpcomingAppointmentDto(
        Guid AppointmentId,
        Guid CustomerId,
        DateTime StartUtc,
        DateTime EndUtc,
        string Notes,
        string CustomerName
    );
}


