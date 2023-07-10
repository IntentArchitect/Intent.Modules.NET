using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ClassWithEnums.CreateClassWithEnums
{
    public class CreateClassWithEnumsCommandValidator : AbstractValidator<CreateClassWithEnumsCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
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