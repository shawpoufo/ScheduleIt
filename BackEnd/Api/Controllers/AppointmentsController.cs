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
            var appointmentId = await _mediator.Send(command, cancellationToken);
            return Created($"/api/appointments/{appointmentId}", new { id = appointmentId });
        }


        [HttpGet("stats/today")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodayStats([FromQuery] DateTime? nowUtc, CancellationToken cancellationToken)
        {
            var query = new GetTodayStatsQuery(nowUtc);
            var dto = await _mediator.Send(query, cancellationToken);
            return Ok(dto);
        }


        [HttpGet("range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppointmentsInRange(
            [FromQuery, Required] DateTime startUtc,
            [FromQuery, Required] DateTime endUtc,
            CancellationToken cancellationToken)
        {
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
            // Always trust the route id and override any body-provided id
            command = command with { AppointmentId = id };

            var newStatus = await _mediator.Send(command, cancellationToken);
            return Ok(new { id, status = newStatus });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([Required] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAppointmentCommand(id), cancellationToken);
            return NoContent();
        }
    }
}