using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Interfaces;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.ValidationMiddleware", Version = "1.0")]

namespace Wolverine.Subscribe.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    public class ValidationMiddleware
    {
        public async Task BeforeAsync(
            Envelope envelope,
            IValidatorProvider validatorProvider,
            CancellationToken cancellationToken)
        {
            if (envelope.Message is null)
            {
                return;
            }
            await ValidateAsync(envelope.Message, validatorProvider, cancellationToken);
        }

        private static async Task ValidateAsync(
            object request,
            IValidatorProvider validatorProvider,
            CancellationToken cancellationToken)
        {
            if (request is IBypassValidation)
            {
                return;
            }
            var validator = GetValidator(request, validatorProvider);

            if (validator is null)
            {
                return;
            }
            var context = new ValidationContext<object>(request);
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            var failures = validationResult.Errors.Where(error => error is not null).ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        private static IValidator? GetValidator(object request, IValidatorProvider validatorProvider)
        {
            var providerMethod = typeof(IValidatorProvider).GetMethod(nameof(IValidatorProvider.GetValidator))!.MakeGenericMethod(request.GetType());
            return providerMethod.Invoke(validatorProvider, null) as IValidator;
        }
    }
}