using System;
using Entities.Constants.TestApplication.Domain.Entities;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.CreateTestClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTestClassCommandValidator : AbstractValidator<CreateTestClassCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateTestClassCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Att100)
                .NotNull()
                .MaximumLength(TestClass.Att100MaxLength);

            RuleFor(v => v.VarChar200)
                .NotNull()
                .MaximumLength(TestClass.VarChar200MaxLength);

            RuleFor(v => v.NVarChar300)
                .NotNull()
                .MaximumLength(TestClass.NVarChar300MaxLength);

            RuleFor(v => v.AttMax)
                .NotNull();

            RuleFor(v => v.VarCharMax)
                .NotNull();

            RuleFor(v => v.NVarCharMax)
                .NotNull();
        }
    }
}