using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNullabilityConstrainedEntityCommandValidator : AbstractValidator<CreateNullabilityConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNullabilityConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RequiredString)
                .NotNull();
        }
    }
}