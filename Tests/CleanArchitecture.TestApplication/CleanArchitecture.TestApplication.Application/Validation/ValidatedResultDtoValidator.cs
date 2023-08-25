using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidatedResultDtoValidator : AbstractValidator<ValidatedResultDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public ValidatedResultDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RangeStr)
                .NotNull()
                .Length(3, 5);

            RuleFor(v => v.MinStr)
                .NotNull()
                .MinimumLength(3);

            RuleFor(v => v.MaxStr)
                .NotNull()
                .MaximumLength(10);

            RuleFor(v => v.RangeInt)
                .InclusiveBetween(0, 10);

            RuleFor(v => v.MinInt)
                .GreaterThanOrEqualTo(0);

            RuleFor(v => v.MaxInt)
                .LessThanOrEqualTo(5);

            RuleFor(v => v.IsRequired)
                .NotNull();

            RuleFor(v => v.IsRequiredEmpty)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.DecimalRange)
                .InclusiveBetween(10, 20);

            RuleFor(v => v.DecimalMin)
                .GreaterThanOrEqualTo(0);

            RuleFor(v => v.DecimalMax)
                .LessThanOrEqualTo(1000);

            RuleFor(v => v.StringOptionNonEmpty)
                .NotEmpty();

            RuleFor(v => v.MyEnum)
                .NotNull()
                .IsInEnum();
        }
    }
}