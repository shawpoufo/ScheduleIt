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
        private readonly ICustomerRepository _customerRepository;

        public GetTodayStatsQueryHandler(IAppointmentRepository appointmentRepository, ICustomerRepository customerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _customerRepository = customerRepository;
        }

        public async Task<TodayStatsDto> Handle(GetTodayStatsQuery request, CancellationToken cancellationToken)
        {
            var total = await _appointmentRepository.CountAllAsync(cancellationToken);
            var todayCount = await _appointmentRepository.CountTodayAsync(request.TodayUtc, cancellationToken);
            var upcoming = await _appointmentRepository.GetUpcomingTodayAsync(request.TodayUtc, 5, cancellationToken);

            var customerIds = upcoming.Select(a => a.CustomerId).Distinct().ToList();
            var customers = await _customerRepository.GetByIdsAsync(customerIds, cancellationToken);
            var customerMap = customers.ToDictionary(c => c.Id, c => c.Name);

            var upcomingDtos = upcoming
                .Select(a => new TodayUpcomingAppointmentDto(
                    a.Id,
                    a.CustomerId,
                    a.TimeSlot.StartUtc,
                    a.TimeSlot.EndUtc,
                    a.Notes,
                    customerMap.TryGetValue(a.CustomerId, out var name) ? name : string.Empty))
                .ToList();

            return new TodayStatsDto(total, todayCount, upcomingDtos);
        }
    }
}


