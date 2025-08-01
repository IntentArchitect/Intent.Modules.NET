using System;
using AzureFunctions.NET6.Application.Params.FromBodyTest;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Params.FromBodyTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FromBodyTestCommandValidator : AbstractValidator<FromBodyTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public FromBodyTestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Ids)
                .NotNull();
        }
    }
}