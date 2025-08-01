using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.CreateTestNullablity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTestNullablityCommandValidator : AbstractValidator<CreateTestNullablityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTestNullablityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MyEnum)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Str)
                .NotNull();

            RuleFor(v => v.NullableEnum)
                .IsInEnum();

            RuleFor(v => v.DefaultLiteralEnum)
                .NotNull()
                .IsInEnum();
        }
    }
}