using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.ExternalMessagePublish.PublishExternalMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PublishExternalMessageCommandValidator : AbstractValidator<PublishExternalMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PublishExternalMessageCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}