using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Customers.Commands.CreateCustomer;
using Application.Customers.Queries.GetCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    /// <summary>
    /// Manages customer operations including creation and retrieval
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="command">The customer creation details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created customer ID</returns>
        /// <response code="201">Customer successfully created</response>
        /// <response code="400">Invalid customer data or validation error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var customerId = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetCustomer), new { id = customerId }, new { id = customerId });
            }
            catch (Application.Common.ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific customer by ID
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The customer details</returns>
        /// <response code="200">Customer found and returned</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomer([Required] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetCustomerQuery(id);
                var customer = await _mediator.Send(query, cancellationToken);
                return Ok(customer);
            }
            catch (Application.Common.NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}

