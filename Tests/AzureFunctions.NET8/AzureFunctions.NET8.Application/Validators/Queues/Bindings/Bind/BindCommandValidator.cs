using System;
using AzureFunctions.NET8.Application.Queues.Bindings.Bind;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.Queues.Bindings.Bind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BindCommandValidator : AbstractValidator<BindCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public BindCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}