using System;
using System.Collections.Generic;
using MediatR;
using Domain.Common;

namespace Application.Appointments.Queries.GetUpcomingAppointments
{
    public sealed record GetUpcomingAppointmentsQuery(DateTime FromUtc) : IRequest<IReadOnlyList<UpcomingAppointmentDto>>;

    public sealed record UpcomingAppointmentDto(Guid AppointmentId, Guid CustomerId, DateTime StartUtc, DateTime EndUtc, AppointmentStatus Status);
}


