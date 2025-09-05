using System;
using Domain.Common;
using MediatR;

namespace Application.Appointments.Commands.UpdateAppointmentStatus
{
    public sealed record UpdateAppointmentStatusCommand(Guid AppointmentId, AppointmentStatus Status) : IRequest<AppointmentStatus>;
}


