using System;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ScheduleIt.Xunit.Domain
{
    public sealed class AppointmentTimeSlotTests
    {
        [Fact]
        public void Create_Should_Throw_When_In_Past()
        {
            // arrange
            var now = DateTime.UtcNow;
            
            // act
            Action act = () => AppointmentTimeSlot.Create(now.AddMinutes(-10), now.AddMinutes(50), now);
            
            // assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*in the past*");
        }

        [Theory]
        [InlineData(10)]
        [InlineData(29)]
        public void Create_Should_Throw_When_Duration_Too_Short(int minutes)
        {
            // arrange
            var now = DateTime.UtcNow;
            var start = now.AddHours(1);
            var end = start.AddMinutes(minutes);
            
            // act
            Action act = () => AppointmentTimeSlot.Create(start, end, now);
            
            // assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*at least 30 minutes*");
        }

        [Fact]
        public void Create_Should_Throw_When_Start_After_End()
        {
            // arrange
            var now = DateTime.UtcNow;
            var start = now.AddHours(2);
            var end = now.AddHours(1);
            
            // act
            Action act = () => AppointmentTimeSlot.Create(start, end, now);
            
            // assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*before end time*");
        }

        [Fact]
        public void IsOverlapping_Should_Return_True_When_Overlap()
        {
            // arrange
            var now = DateTime.UtcNow;
            var a = AppointmentTimeSlot.CreateFromStartAndEnd(now, now.AddHours(2));
            
            // act
            var result = a.IsOverlapping(now.AddHours(1), now.AddHours(3));
            
            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOverlapping_Should_Return_False_When_No_Overlap()
        {
            // arrange
            var now = DateTime.UtcNow;
            var a = AppointmentTimeSlot.CreateFromStartAndEnd(now, now.AddHours(2));
            
            // act
            var result = a.IsOverlapping(now.AddHours(2), now.AddHours(3));
            
            // assert
            result.Should().BeFalse();
        }
    }
}


