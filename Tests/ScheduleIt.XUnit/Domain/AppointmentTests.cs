using System;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ScheduleIt.Xunit.Domain
{
    public sealed class AppointmentTests
    {
        [Fact]
        public void Create_Should_Set_Initial_State_And_Raise_Booked_Event()
        {
            // arrange
            var customerId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var start = now.AddHours(1);
            var end = start.AddHours(1);

            // act
            var appt = Appointment.Create(customerId, start, end, now, "notes");

            // assert
            appt.CustomerId.Should().Be(customerId);
            appt.Status.Should().Be(AppointmentStatus.Scheduled);
            appt.Notes.Should().Be("notes");
            appt.TimeSlot.StartUtc.Should().Be(start);
            appt.TimeSlot.EndUtc.Should().Be(end);
            appt.DomainEvents.Should().NotBeEmpty();
        }

        [Fact]
        public void Cancel_Before_Start_Should_Set_Status_To_Canceled_And_Raise_Event()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(2), now.AddHours(3), now);

            // act
            appt.Cancel(now.AddHours(1));

            // assert
            appt.Status.Should().Be(AppointmentStatus.Canceled);
            appt.DomainEvents.Should().NotBeEmpty();
        }

        [Fact]
        public void Cancel_After_Start_Should_Throw_DomainRuleViolationException()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddMinutes(30), now.AddHours(2), now);

            // act
            Action act = () => appt.Cancel(now.AddHours(1));
            
            // assert
            act.Should().Throw<DomainRuleViolationException>();
        }

        [Fact]
        public void MarkAsCompleted_From_Scheduled_Should_Set_Status()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);

            // act
            appt.MarkAsCompleted();
            
            // assert
            appt.Status.Should().Be(AppointmentStatus.Completed);
        }

        [Fact]
        public void MarkAsInProgress_From_Scheduled_Should_Set_Status()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);

            // act
            appt.MarkAsInProgress();
            
            // assert
            appt.Status.Should().Be(AppointmentStatus.InProgress);
        }

        [Fact]
        public void MarkAsNoShow_From_Scheduled_Should_Set_Status()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);

            // act
            appt.MarkAsNoShow();
            
            // assert
            appt.Status.Should().Be(AppointmentStatus.NoShow);
        }

        [Fact]
        public void Invalid_Transitions_Should_Throw_DomainRuleViolationException()
        {
            // arrange
            var now = DateTime.UtcNow;
            var appt = Appointment.Create(Guid.NewGuid(), now.AddHours(1), now.AddHours(2), now);

            // act
            appt.MarkAsCompleted();

            // assert
            Action toInProgress = () => appt.MarkAsInProgress();
            toInProgress.Should().Throw<DomainRuleViolationException>();

            Action toNoShow = () => appt.MarkAsNoShow();
            toNoShow.Should().Throw<DomainRuleViolationException>();
        }
    }
}


