using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAnemicIntegrationTriggeringCommandValidator : AbstractValidator<CreateAnemicIntegrationTriggeringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAnemicIntegrationTriggeringCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}