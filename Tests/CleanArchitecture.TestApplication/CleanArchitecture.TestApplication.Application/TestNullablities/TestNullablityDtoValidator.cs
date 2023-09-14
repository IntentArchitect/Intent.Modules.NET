using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestNullablityDtoValidator : AbstractValidator<TestNullablityDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public TestNullablityDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MyEnum)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Str)
                .NotNull();

            RuleFor(v => v.NullableEnum)
                .IsInEnum();
        }
    }
}