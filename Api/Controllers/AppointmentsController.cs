using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Appointments.Commands.BookAppointment;
using Application.Appointments.Commands.CancelAppointment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

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
        /// <param name="command">The appointment booking details</param>
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
            catch (Application.Common.ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Application.Common.NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cancels an existing appointment
        /// </summary>
        /// <param name="id">The appointment ID to cancel</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on successful cancellation</returns>
        /// <response code="204">Appointment successfully canceled</response>
        /// <response code="400">Invalid appointment data or validation error</response>
        /// <response code="404">Appointment not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelAppointment(
            [Required] Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new CancelAppointmentCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Application.Common.ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Application.Common.NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
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
        /// Retrieves upcoming appointments from a specified date
        /// </summary>
        /// <param name="fromUtc">Start date for upcoming appointments (UTC)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of upcoming appointments</returns>
        /// <response code="200">Upcoming appointments retrieved successfully</response>
        /// <response code="400">Invalid date parameter</response>
        [HttpGet("upcoming")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUpcomingAppointments(
            [FromQuery, Required] DateTime fromUtc,
            CancellationToken cancellationToken)
        {
            // TODO: Implement GetUpcomingAppointments query handler
            return Ok(new { message = "GetUpcomingAppointments not implemented yet" });
        }

        /// <summary>
        /// Retrieves all appointments for a specific customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of customer appointments</returns>
        /// <response code="200">Customer appointments retrieved successfully</response>
        /// <response code="400">Invalid customer ID</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerAppointments(
            [Required] Guid customerId,
            CancellationToken cancellationToken)
        {
            // TODO: Implement GetCustomerAppointments query handler
            return Ok(new { message = "GetCustomerAppointments not implemented yet" });
        }
    }
}

