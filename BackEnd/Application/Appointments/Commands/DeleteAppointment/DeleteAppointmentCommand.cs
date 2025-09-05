using System;
using MediatR;

namespace Application.Appointments.Commands.DeleteAppointment
{
    public sealed record DeleteAppointmentCommand(Guid Id) : IRequest;
}


