using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManyDest
{
    public class OperationOneToManyDestCommandValidator : AbstractValidator<OperationOneToManyDestCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public OperationOneToManyDestCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}