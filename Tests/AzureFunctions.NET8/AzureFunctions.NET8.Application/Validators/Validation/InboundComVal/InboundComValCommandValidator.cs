using System;
using System.Text.RegularExpressions;
using AzureFunctions.NET8.Application.Validation.InboundComVal;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.Validation.InboundComVal
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InboundComValCommandValidator : AbstractValidator<InboundComValCommand>
    {
        private static readonly Regex RegexFieldRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        [IntentManaged(Mode.Merge)]
        public InboundComValCommandValidator()
        {
            ConfigureValidationRules();
        }

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

            RuleFor(v => v.RegexField)
                .NotNull()
                .Matches(RegexFieldRegex);
        }
    }
}