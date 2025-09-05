using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using MediatR;

namespace Application.Appointments.Queries.GetTodayStats
{
    public sealed class GetTodayStatsQueryHandler : IRequestHandler<GetTodayStatsQuery, TodayStatsDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetTodayStatsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<TodayStatsDto> Handle(GetTodayStatsQuery request, CancellationToken cancellationToken)
        {
            var total = await _appointmentRepository.CountAllAsync(cancellationToken);
            var todayCount = await _appointmentRepository.CountTodayAsync(request.TodayUtc, cancellationToken);
            var upcoming = await _appointmentRepository.GetUpcomingTodayAsync(request.TodayUtc, 5, cancellationToken);

            var upcomingDtos = upcoming
                .Select(a => new TodayUpcomingAppointmentDto(a.Id, a.CustomerId, a.TimeSlot.StartUtc, a.TimeSlot.EndUtc, a.Notes))
                .ToList();

            return new TodayStatsDto(total, todayCount, upcomingDtos);
        }
    }
}


