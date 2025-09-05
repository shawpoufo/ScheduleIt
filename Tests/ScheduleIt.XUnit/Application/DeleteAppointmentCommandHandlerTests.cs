using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Appointments.Commands.DeleteAppointment;
using Application.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ScheduleIt.XUnit.Application
{
    public sealed class DeleteAppointmentCommandHandlerTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        [Fact]
        public async Task Handle_Should_Delete_When_Found()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            // act
            var handler = new DeleteAppointmentCommandHandler(_appointmentRepo.Object, _uow.Object);
            await handler.Handle(new DeleteAppointmentCommand(appt.Id), CancellationToken.None);

            // assert
            _appointmentRepo.Verify(r => r.Remove(appt), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFound_When_Missing()
        {
            // arrange
            var handler = new DeleteAppointmentCommandHandler(_appointmentRepo.Object, _uow.Object);

            // act
            Func<Task> act = () => handler.Handle(new DeleteAppointmentCommand(Guid.NewGuid()), CancellationToken.None);

            // assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Should_Throw_DomainRule_When_Not_Scheduled()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);
            appt.MarkAsInProgress();
            _appointmentRepo.Setup(r => r.GetByIdAsync(appt.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appt);

            var handler = new DeleteAppointmentCommandHandler(_appointmentRepo.Object, _uow.Object);

            // act
            Func<Task> act = () => handler.Handle(new DeleteAppointmentCommand(appt.Id), CancellationToken.None);

            // assert
            await act.Should().ThrowAsync<Domain.Common.DomainRuleViolationException>();
            _appointmentRepo.Verify(r => r.Remove(It.IsAny<Appointment>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}


