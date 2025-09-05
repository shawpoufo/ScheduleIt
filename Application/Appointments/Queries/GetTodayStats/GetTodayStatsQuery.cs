using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Appointments.Queries.GetTodayStats
{
    public sealed record GetTodayStatsQuery(DateTime TodayUtc) : IRequest<TodayStatsDto>;

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


