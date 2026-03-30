using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNullabilityConstrainedEntityCommandValidator : AbstractValidator<UpdateNullabilityConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNullabilityConstrainedEntityCommandValidator()
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