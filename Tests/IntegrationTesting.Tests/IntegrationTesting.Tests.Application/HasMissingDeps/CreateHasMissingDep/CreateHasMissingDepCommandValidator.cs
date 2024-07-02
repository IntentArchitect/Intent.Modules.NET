using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.CreateHasMissingDep
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateHasMissingDepCommandValidator : AbstractValidator<CreateHasMissingDepCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateHasMissingDepCommandValidator()
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