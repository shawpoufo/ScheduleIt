using FluentValidation;

namespace Application.Appointments.Commands.UpdateAppointmentStatus
{
    public sealed class UpdateAppointmentStatusCommandValidator : AbstractValidator<UpdateAppointmentStatusCommand>
    {
        public UpdateAppointmentStatusCommandValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty();

            RuleFor(x => x.Status)
                .IsInEnum();
        }
    }
}


