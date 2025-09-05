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
                return Created($"/api/appointments/{appointmentId}", new { id = appointmentId });
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }


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