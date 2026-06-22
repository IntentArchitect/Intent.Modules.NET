using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AwsLambdaFunctions.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.ValidationMiddleware", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Infrastructure.Dispatch.Middleware
{
    public class ValidationMiddleware
    {
        public async Task BeforeAsync(
            Envelope envelope,
            IValidatorProvider validatorProvider,
            CancellationToken cancellationToken)
        {
            await ValidateAsync(envelope.Message, validatorProvider, cancellationToken);
        }

        private static async Task ValidateAsync(
            object request,
            IValidatorProvider validatorProvider,
            CancellationToken cancellationToken)
        {
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