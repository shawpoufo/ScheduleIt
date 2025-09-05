using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Customers.Commands.CreateCustomer;
using Application.Customers.Queries.GetCustomer;
using Application.Customers.Queries.SearchCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
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
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchCustomers([FromQuery] string? search, CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchCustomersQuery(search);
                var customers = await _mediator.Send(query, cancellationToken);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }

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
            catch (Exception ex)
            {
                return ProblemDetailsMapper.Map(ex);
            }
        }
    }
}

