using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateAnemicIntegrationTriggeringCommandValidator : AbstractValidator<UpdateAnemicIntegrationTriggeringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateAnemicIntegrationTriggeringCommandValidator()
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