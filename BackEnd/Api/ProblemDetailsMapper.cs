using Application.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    public static class ProblemDetailsMapper
    {
        public static IActionResult Map(Exception ex)
        {
            return ex switch
            {
                NotFoundException nf => new NotFoundObjectResult(new { error = nf.Message }),
                DomainRuleViolationException drv => new BadRequestObjectResult(new { error = drv.Message }),
                ValidationException v => new BadRequestObjectResult(new { error = v.Message }),
                _ => new ObjectResult(new { error = "An unexpected error occurred." }) { StatusCode = 500 }
            };
        }
    }
}


