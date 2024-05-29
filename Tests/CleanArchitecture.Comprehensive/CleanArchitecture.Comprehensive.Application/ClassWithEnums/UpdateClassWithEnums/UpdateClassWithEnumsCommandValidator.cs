using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateClassWithEnumsCommandValidator : AbstractValidator<UpdateClassWithEnumsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClassWithEnumsCommandValidator()
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