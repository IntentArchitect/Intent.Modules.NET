using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.ClassWithEnums
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassWithEnumsDtoValidator : AbstractValidator<ClassWithEnumsDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public ClassWithEnumsDtoValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.EnumWithDefaultLiteral)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumWithoutDefaultLiteral)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.EnumWithoutValues)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.NullibleEnumWithDefaultLiteral)
                .IsInEnum();

            RuleFor(v => v.NullibleEnumWithoutDefaultLiteral)
                .IsInEnum();

            RuleFor(v => v.NullibleEnumWithoutValues)
                .IsInEnum();
        }
    }
}