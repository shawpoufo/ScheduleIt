using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Appointments.Queries.GetAppointmentsInRange;
using Application.Appointments.Queries.GetTodayStats;
using Application.Customers.Queries.GetCustomer;
using Application.Customers.Queries.SearchCustomers;
using Application.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace ScheduleIt.XUnit.Application
{
    public sealed class QueriesTests
    {
        [Fact]
        public async Task GetTodayStats_Should_Map_Data_From_Repository()
        {
            // arrange
            var repo = new Mock<IAppointmentRepository>();
            var today = DateTime.UtcNow.Date;
            repo.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
            repo.Setup(r => r.CountTodayAsync(today, It.IsAny<CancellationToken>())).ReturnsAsync(3);

            var upcoming = new List<Appointment>
            {
                Appointment.Create(Guid.NewGuid(), today.AddHours(10), today.AddHours(11), DateTime.UtcNow.AddDays(-1), "A"),
                Appointment.Create(Guid.NewGuid(), today.AddHours(12), today.AddHours(13), DateTime.UtcNow.AddDays(-1), "B"),
            };
            repo.Setup(r => r.GetUpcomingTodayAsync(today, 5, It.IsAny<CancellationToken>())).ReturnsAsync(upcoming);

            // act
            var handler = new GetTodayStatsQueryHandler(repo.Object);
            var result = await handler.Handle(new GetTodayStatsQuery(today), CancellationToken.None);

            // assert
            result.TotalAppointments.Should().Be(10);
            result.TodayAppointments.Should().Be(3);
            result.UpcomingToday.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAppointmentsInRange_Should_Throw_When_Invalid_Range()
        {
            // arrange
            var repo = new Mock<IAppointmentRepository>();
            var handler = new GetAppointmentsInRangeQueryHandler(repo.Object);
            
            // act
            Func<Task> act = () => handler.Handle(new GetAppointmentsInRangeQuery(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1)), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task GetAppointmentsInRange_Should_Map_Results()
        {
            // arrange
            var repo = new Mock<IAppointmentRepository>();
            var start = DateTime.UtcNow;
            var end = start.AddDays(1);
            var appt = Appointment.Create(Guid.NewGuid(), start.AddHours(1), start.AddHours(2), DateTime.UtcNow.AddDays(-1), "hi");
            repo.Setup(r => r.GetInRangeAsync(start, end, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Appointment> { appt });

            // act
            var handler = new GetAppointmentsInRangeQueryHandler(repo.Object);
            var result = await handler.Handle(new GetAppointmentsInRangeQuery(start, end), CancellationToken.None);

            // assert
            result.Should().HaveCount(1);
            var dto = result.First();
            dto.AppointmentId.Should().Be(appt.Id);
            dto.Status.Should().Be(appt.Status);
        }

        [Fact]
        public async Task GetCustomer_Should_Throw_NotFound_When_Missing()
        {
            // arrange
            var repo = new Mock<ICustomerRepository>();
            var handler = new GetCustomerQueryHandler(repo.Object);
            
            // act
            Func<Task> act = () => handler.Handle(new GetCustomerQuery(Guid.NewGuid()), CancellationToken.None);
            
            // assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task SearchCustomers_Should_Map_Results()
        {
            // arrange
            var repo = new Mock<ICustomerRepository>();
            repo.Setup(r => r.SearchAsync("john", It.IsAny<CancellationToken>())).ReturnsAsync(new[]
            {
                new Customer(Guid.NewGuid(), "John Doe", "john@x.com"),
                new Customer(Guid.NewGuid(), "Johnny", "johnny@y.com"),
            });

            // act
            var handler = new SearchCustomersQueryHandler(repo.Object);
            var result = await handler.Handle(new SearchCustomersQuery("john"), CancellationToken.None);

            // assert
            result.Should().HaveCount(2);
        }
    }
}


