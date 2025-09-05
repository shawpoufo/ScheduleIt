using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Appointments.Queries.GetAppointmentsInRange
{
    public sealed class GetAppointmentsInRangeQueryHandler : IRequestHandler<GetAppointmentsInRangeQuery, IReadOnlyList<AppointmentInRangeDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentsInRangeQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IReadOnlyList<AppointmentInRangeDto>> Handle(GetAppointmentsInRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.StartUtc >= request.EndUtc)
                throw new ValidationException("StartUtc must be before EndUtc.");

            var list = await _appointmentRepository.GetInRangeAsync(request.StartUtc, request.EndUtc, cancellationToken);

            return list
                .Select(a => new AppointmentInRangeDto(
                    a.Id,
                    a.CustomerId,
                    a.TimeSlot.StartUtc,
                    a.TimeSlot.EndUtc,
                    a.Status,
                    a.Notes
                ))
                .ToList();
        }
    }
}


