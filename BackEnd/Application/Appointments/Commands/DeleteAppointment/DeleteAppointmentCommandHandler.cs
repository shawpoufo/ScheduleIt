using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Appointments.Commands.DeleteAppointment
{
    public sealed class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (appointment == null)
                throw new NotFoundException($"Appointment '{request.Id}' not found.");

            appointment.EnsureCanBeDeleted();
            _appointmentRepository.Remove(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}


