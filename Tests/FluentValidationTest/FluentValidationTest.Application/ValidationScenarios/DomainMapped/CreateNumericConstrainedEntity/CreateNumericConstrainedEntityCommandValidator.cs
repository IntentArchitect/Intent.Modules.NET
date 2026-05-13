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
                .InclusiveBetween(0d, 100d);

            RuleFor(v => v.Price)
                .InclusiveBetween(1m, 10000m);

            RuleFor(v => v.ExclusiveMinInclusiveMaxFloat)
                .GreaterThan(0f)
                .LessThanOrEqualTo(100f);

            RuleFor(v => v.InclusiveMinExclusiveMaxDouble)
                .GreaterThanOrEqualTo(10d)
                .LessThan(50d);

            RuleFor(v => v.ExclusiveMinExclusiveMaxDecimal)
                .ExclusiveBetween(0.01m, 999.99m);

            RuleFor(v => v.OnlyMinExclusiveFloat)
                .GreaterThan(-10.5f);

            RuleFor(v => v.OnlyMaxExclusiveDouble)
                .LessThan(1000.0d);

            RuleFor(v => v.InclusiveMinInclusiveMaxFloat)
                .InclusiveBetween(5.5f, 99.9f);

            RuleFor(v => v.NegativeRangeDecimal)
                .GreaterThanOrEqualTo(-100.00m)
                .LessThan(-0.01m);

            RuleFor(v => v.NarrowRangeDouble)
                .ExclusiveBetween(0.001d, 0.999d);
        }
    }
}