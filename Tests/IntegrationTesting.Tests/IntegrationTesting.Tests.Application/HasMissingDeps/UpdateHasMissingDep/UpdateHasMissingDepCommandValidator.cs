using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.UpdateHasMissingDep
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateHasMissingDepCommandValidator : AbstractValidator<UpdateHasMissingDepCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateHasMissingDepCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}