using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.DeleteDerived
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDerivedCommandValidator : AbstractValidator<DeleteDerivedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDerivedCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}