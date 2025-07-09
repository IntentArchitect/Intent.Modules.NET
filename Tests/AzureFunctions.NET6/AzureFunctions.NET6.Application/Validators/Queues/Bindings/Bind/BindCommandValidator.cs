using System;
using AzureFunctions.NET6.Application.Queues.Bindings.Bind;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Queues.Bindings.Bind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BindCommandValidator : AbstractValidator<BindCommand>
    {
        [IntentManaged(Mode.Merge)]
        public BindCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}