using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Common;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Appointments.Commands.UpdateAppointmentStatus
{
    public sealed class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, AppointmentStatus>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAppointmentStatusCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentStatus> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment == null)
                throw new NotFoundException($"Appointment '{request.AppointmentId}' not found.");

            switch (request.Status)
            {
                case AppointmentStatus.Scheduled:
                    appointment.MarkAsScheduled();
                    break;

                case AppointmentStatus.InProgress:
                    appointment.MarkAsInProgress();
                    break;
                case AppointmentStatus.Completed:
                    appointment.MarkAsCompleted();
                    break;
                case AppointmentStatus.Canceled:
                    appointment.Cancel(DateTime.UtcNow);
                    break;
                case AppointmentStatus.NoShow:
                    appointment.MarkAsNoShow();
                    break;
                default:
                    throw new ValidationException("Unsupported status transition.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return appointment.Status;
        }
    }
}


