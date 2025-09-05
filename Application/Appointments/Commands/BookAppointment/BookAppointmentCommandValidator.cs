using FluentValidation;

namespace Application.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
    {
        public BookAppointmentCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.StartUtc).NotEmpty();
            RuleFor(x => x.EndUtc).NotEmpty();
            RuleFor(x => x).Must(x => x.StartUtc < x.EndUtc)
                .WithMessage("StartUtc must be before EndUtc.");
            RuleFor(x => (x.EndUtc - x.StartUtc).TotalMinutes)
                .GreaterThanOrEqualTo(30).WithMessage("Duration must be at least 30 minutes.")
                .LessThanOrEqualTo(12 * 60).WithMessage("Duration cannot exceed 12 hours.");
        }
    }
}


