using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNumericConstrainedEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNumericConstrainedEntityCommandValidator : AbstractValidator<CreateNumericConstrainedEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNumericConstrainedEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Age)
                .InclusiveBetween(18, 120);

            RuleFor(v => v.Percentage)
                .InclusiveBetween(0, 100);

            RuleFor(v => v.Score)
                .InclusiveBetween(0, 100);

            RuleFor(v => v.Price)
                .InclusiveBetween(1m, 10000m);
        }
    }
}