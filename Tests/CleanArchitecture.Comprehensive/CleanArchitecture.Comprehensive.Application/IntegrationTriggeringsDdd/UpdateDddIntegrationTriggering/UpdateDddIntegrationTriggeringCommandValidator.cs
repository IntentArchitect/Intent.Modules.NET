using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsDdd.UpdateDddIntegrationTriggering
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDddIntegrationTriggeringCommandValidator : AbstractValidator<UpdateDddIntegrationTriggeringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDddIntegrationTriggeringCommandValidator()
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