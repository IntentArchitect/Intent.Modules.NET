using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.CreateClassWithEnums
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClassWithEnumsCommandValidator : AbstractValidator<CreateClassWithEnumsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateClassWithEnumsCommandValidator()
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

            RuleFor(v => v.CollectionEnum)
                .NotNull()
                .ForEach(x => x.IsInEnum());

            RuleFor(v => v.CollectionStrings)
                .NotNull();
        }
    }
}