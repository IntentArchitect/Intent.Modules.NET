using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.UpdateDerived
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDerivedCommandValidator : AbstractValidator<UpdateDerivedCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public UpdateDerivedCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.BaseAttribute)
                .NotNull();
        }
    }
}