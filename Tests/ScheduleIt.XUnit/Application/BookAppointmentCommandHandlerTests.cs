using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Appointments.Commands.BookAppointment;
using Application.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ScheduleIt.XUnit.Application
{
    public sealed class BookAppointmentCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepo = new();
        private readonly Mock<IAppointmentRepository> _appointmentRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        [Fact]
        public async Task Handle_Should_Create_Appointment_When_Valid()
        {
            // arrange
            var customerId = Guid.NewGuid();
            _customerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer(customerId, "John", "john@doe.com"));
            _appointmentRepo.Setup(r => r.HasOverlappingAppointmentsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new BookAppointmentCommandHandler(_customerRepo.Object, _appointmentRepo.Object, _uow.Object);
            var cmd = new BookAppointmentCommand(customerId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), "notes");

            // act
            var id = await handler.Handle(cmd, CancellationToken.None);

            // assert
            id.Should().NotBeEmpty();
            _appointmentRepo.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFound_When_Customer_Missing()
        {
            // arrange
            var customerId = Guid.NewGuid();
            _customerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            var handler = new BookAppointmentCommandHandler(_customerRepo.Object, _appointmentRepo.Object, _uow.Object);
            var cmd = new BookAppointmentCommand(customerId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

            // act
            Func<Task> act = () => handler.Handle(cmd, CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_Validation_When_Overlap()
        {
            // arrange
            var customerId = Guid.NewGuid();
            _customerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Customer(customerId, "John", "john@doe.com"));
            _appointmentRepo.Setup(r => r.HasOverlappingAppointmentsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new BookAppointmentCommandHandler(_customerRepo.Object, _appointmentRepo.Object, _uow.Object);
            var cmd = new BookAppointmentCommand(customerId, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

            // act
            Func<Task> act = () => handler.Handle(cmd, CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}


