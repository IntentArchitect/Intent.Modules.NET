using CqrsSpike.Subscriber.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Mediator;

namespace CqrsSpike.Subscriber.Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IMessage
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is IBypassPipelineValidation)
            {
                return await next(request, cancellationToken);
            }

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = new List<ValidationFailure>();

                // One validator at a time: async rules share the scoped DbContext, which does not
                // support concurrent operations.
                foreach (var validator in _validators)
                {
                    var result = await validator.ValidateAsync(context, cancellationToken);
                    failures.AddRange(result.Errors.Where(f => f != null));
                }

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next(request, cancellationToken);
        }
    }
}