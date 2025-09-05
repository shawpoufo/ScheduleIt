using Application.Common;
using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

                if (failures.Count > 0)
                {
                    var message = string.Join("; ", failures.Select(f => f.ErrorMessage).Distinct());
                    throw new Application.Common.ValidationException(message);
                }
            }

            return await next();
        }
    }
}


