using System;
using System.Collections.Generic;
using MediatR;
using Domain.Common;

namespace Application.Appointments.Queries.GetAppointmentsInRange
{
    public sealed record GetAppointmentsInRangeQuery(DateTime StartUtc, DateTime EndUtc) : IRequest<IReadOnlyList<AppointmentInRangeDto>>;

    public sealed record AppointmentInRangeDto(Guid AppointmentId, Guid CustomerId, DateTime StartUtc, DateTime EndUtc, AppointmentStatus Status, string Notes);
}


