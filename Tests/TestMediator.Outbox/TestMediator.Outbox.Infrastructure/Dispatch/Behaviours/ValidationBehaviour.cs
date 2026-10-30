using FluentValidation;
using FluentValidation.Results;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Outbox.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.ValidationBehaviour", Version = "1.0")]

namespace TestMediator.Outbox.Infrastructure.Dispatch.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : global::Mediator.IMessage
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
            if (request is IBypassValidation)
            {
                return await next(request, cancellationToken);
            }

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = new List<ValidationFailure>();

                // One validator at a time: async rules (such as unique constraint checks) share the scoped DbContext, which does not support concurrent operations.

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