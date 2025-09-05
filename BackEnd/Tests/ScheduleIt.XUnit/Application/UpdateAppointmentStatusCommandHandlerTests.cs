using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Appointments.Commands.UpdateAppointmentStatus;
using Application.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ScheduleIt.XUnit.Application
{
    public sealed class UpdateAppointmentStatusCommandHandlerTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        [Fact]
        public async Task Handle_Should_Set_InProgress()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            await handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.InProgress), CancellationToken.None);

            // assert
            appt.Status.Should().Be(AppointmentStatus.InProgress);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Set_Completed()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            await handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.Completed), CancellationToken.None);

            // assert
            appt.Status.Should().Be(AppointmentStatus.Completed);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Cancel_Appointment_When_Status_Is_Canceled()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(2), now.AddHours(3), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            await handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.Canceled), CancellationToken.None);

            // assert
            appt.Status.Should().Be(AppointmentStatus.Canceled);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Set_NoShow()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            await handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.NoShow), CancellationToken.None);

            // assert
            appt.Status.Should().Be(AppointmentStatus.NoShow);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Canceling_Already_Started_Appointment()
        {
            // arrange
            var now = DateTime.UtcNow;
            // Create appointment that has already started (start time is in the past)
            var appt = Appointment.Create(Guid.NewGuid(), now.AddMinutes(-30), now.AddHours(1), now.AddMinutes(-60));
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            Func<Task> act = () => handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.Canceled), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<DomainRuleViolationException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Invalid_Transition_From_Completed()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            appt.MarkAsCompleted();
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            Func<Task> act = () => handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.InProgress), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<DomainRuleViolationException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Invalid_Transition_From_Canceled()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            appt.Cancel(now.AddMinutes(30));
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            Func<Task> act = () => handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, AppointmentStatus.Completed), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<DomainRuleViolationException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFound_When_Missing()
        {
            // arrange
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            
            // act
            Func<Task> act = () => handler.Handle(new UpdateAppointmentStatusCommand(Guid.NewGuid(), AppointmentStatus.Completed), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_Validation_When_Unsupported_Status()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new UpdateAppointmentStatusCommandHandler(_appointmentRepo.Object, _uow.Object);
            Func<Task> act = () => handler.Handle(new UpdateAppointmentStatusCommand(appt.Id, (AppointmentStatus)999), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}