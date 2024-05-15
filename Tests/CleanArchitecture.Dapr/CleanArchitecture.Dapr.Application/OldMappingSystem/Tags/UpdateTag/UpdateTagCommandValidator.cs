using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.UpdateTag
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateTagCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}