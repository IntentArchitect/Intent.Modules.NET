using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.CreateDerived
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDerivedCommandValidator : AbstractValidator<CreateDerivedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDerivedCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();

            RuleFor(v => v.BaseAttribute)
                .NotNull();
        }
    }
}