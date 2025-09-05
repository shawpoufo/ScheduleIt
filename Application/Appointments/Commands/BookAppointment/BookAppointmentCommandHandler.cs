using System;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Application.Common;
using Application.Abstractions;

namespace Application.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookAppointmentCommandHandler(
            ICustomerRepository customerRepository,
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Check if customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID '{request.CustomerId}' not found.");
            }

            // Check for overlapping appointments using supplied end time
                var hasOverlap = await _appointmentRepository.HasOverlappingAppointmentsAsync(request.StartUtc, request.EndUtc, cancellationToken);
            
            if (hasOverlap)
            {
                throw new ValidationException("This time slot overlaps with an existing appointment.");
            }

            // Create appointment with domain invariants (domain will validate business rules)
            var appointment = Appointment.Create(request.CustomerId, request.StartUtc, request.EndUtc, now, request.Notes);

            _appointmentRepository.Add(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}
