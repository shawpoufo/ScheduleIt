using System;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Application.Common;
using Application.Abstractions;

namespace Application.Appointments.Commands.CancelAppointment
{
    public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

            if (appointment == null)
            {
                throw new NotFoundException($"Appointment with ID '{request.AppointmentId}' not found.");
            }

            var now = DateTime.UtcNow;

            // Cancel appointment with domain invariants (domain will validate business rules)
            appointment.Cancel(now);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
