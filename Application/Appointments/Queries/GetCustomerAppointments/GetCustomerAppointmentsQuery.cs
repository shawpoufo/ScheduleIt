using System;
using System.Collections.Generic;
using MediatR;
using Domain.Common;

namespace Application.Appointments.Queries.GetCustomerAppointments
{
    public sealed record GetCustomerAppointmentsQuery(Guid CustomerId) : IRequest<IReadOnlyList<CustomerAppointmentDto>>;

    public sealed record CustomerAppointmentDto(Guid AppointmentId, DateTime StartUtc, DateTime EndUtc, AppointmentStatus Status);
}


