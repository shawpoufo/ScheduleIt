using System;
using MediatR;

namespace Application.Appointments.Commands.BookAppointment
{
    public sealed record BookAppointmentCommand(Guid CustomerId, DateTime StartUtc, DateTime EndUtc, string? Notes = null) : IRequest<Guid>;
}


