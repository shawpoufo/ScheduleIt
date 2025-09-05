using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Appointments.Commands.BookAppointment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Application.Appointments.Queries.GetAppointmentsInRange;
using Application.Appointments.Commands.UpdateAppointmentStatus;
using Application.Appointments.Queries.GetTodayStats;
using Application.Appointments.Commands.DeleteAppointment;

namespace Api.Controllers
{
    /// <summary>
    /// Manages appointment operations including booking, cancellation, and retrieval
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Books a new appointment for a customer
        /// </summary>
        /// <param name="command">The appointment booking details (CustomerId, StartUtc, EndUtc, Notes)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created appointment ID</returns>
        /// <response code="201">Appointment successfully booked</response>
        /// <response code="400">Invalid appointment data or validation error</response>
        /// <response code="404">Customer not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BookAppointment(
            [FromBody] BookAppointmentCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var appointmentId = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointmentId }, new { id = appointmentId });
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }


        /// <summary>
        /// Retrieves a specific appointment by ID
        /// </summary>
        /// <param name="id">The appointment ID</param>
        /// <returns>The appointment details</returns>
        /// <response code="200">Appointment found and returned</response>
        /// <response code="404">Appointment not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointment([Required] Guid id)
        {
            // TODO: Implement GetAppointment query handler
            return Ok(new { message = "GetAppointment not implemented yet" });
        }


        /// <summary>
        /// Retrieves dashboard stats for today: total count, today's count, and 5 upcoming today
        /// </summary>
        [HttpGet("stats/today")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodayStats([FromQuery] DateTime? nowUtc, CancellationToken cancellationToken)
        {
            try
            {
                var now = nowUtc ?? DateTime.UtcNow;
                var query = new GetTodayStatsQuery(now);
                var dto = await _mediator.Send(query, cancellationToken);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }


        /// <summary>
        /// Retrieves appointments overlapping a date-time range (any status)
        /// </summary>
        /// <param name="startUtc">Range start (UTC)</param>
        /// <param name="endUtc">Range end (UTC)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of appointments in the range</returns>
        /// <response code="200">Appointments retrieved successfully</response>
        /// <response code="400">Invalid date parameters</response>
        [HttpGet("range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppointmentsInRange(
            [FromQuery, Required] DateTime startUtc,
            [FromQuery, Required] DateTime endUtc,
            CancellationToken cancellationToken)
        {
            if (startUtc == default || endUtc == default)
                return BadRequest(new { error = "startUtc and endUtc are required" });
            if (startUtc >= endUtc)
                return BadRequest(new { error = "startUtc must be before endUtc" });

            var query = new GetAppointmentsInRangeQuery(startUtc, endUtc);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Updates the status of an appointment
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <param name="command">New status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Status updated, returns new status</response>
        /// <response code="400">Invalid status or transition</response>
        /// <response code="404">Appointment not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus([Required] Guid id, [FromBody] UpdateAppointmentStatusCommand command, CancellationToken cancellationToken)
        {
            if (command.AppointmentId == Guid.Empty)
            {
                command = command with { AppointmentId = id };
            }

            if (command.AppointmentId != id)
                return BadRequest(new { error = "AppointmentId mismatch" });

            try
            {
                var newStatus = await _mediator.Send(command, cancellationToken);
                return Ok(new { id, status = newStatus });
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }

        /// <summary>
        /// Permanently deletes an appointment
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">Appointment deleted</response>
        /// <response code="404">Appointment not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DeleteAppointmentCommand(id), cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }
    }
}