using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandNoParam
{
    public class CommandNoParamValidator : AbstractValidator<CommandNoParam>
    {
        [IntentManaged(Mode.Merge)]
        public CommandNoParamValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}