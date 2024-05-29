using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsDdd.CreateDddIntegrationTriggering
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDddIntegrationTriggeringCommandValidator : AbstractValidator<CreateDddIntegrationTriggeringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDddIntegrationTriggeringCommandValidator()
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