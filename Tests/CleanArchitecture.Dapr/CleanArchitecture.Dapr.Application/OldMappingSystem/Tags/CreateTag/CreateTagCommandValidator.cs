using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.CreateTag
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateTagCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}