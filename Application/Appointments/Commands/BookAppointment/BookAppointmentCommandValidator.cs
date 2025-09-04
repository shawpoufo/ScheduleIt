using FluentValidation;

namespace Application.Appointments.Commands.BookAppointment
{
    public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
    {
        public BookAppointmentCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.StartUtc).NotEmpty();
        }
    }
}


