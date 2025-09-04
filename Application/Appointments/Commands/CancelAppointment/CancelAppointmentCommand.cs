using System;
using MediatR;

namespace Application.Appointments.Commands.CancelAppointment
{
    public sealed record CancelAppointmentCommand(Guid AppointmentId) : IRequest;
}


