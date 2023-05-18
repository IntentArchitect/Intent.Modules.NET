using System;
using Entities.Constants.TestApplication.Domain.Entities;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.TestValidation
{
    public class TestValidationQueryValidator : AbstractValidator<TestValidationQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public TestValidationQueryValidator()
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