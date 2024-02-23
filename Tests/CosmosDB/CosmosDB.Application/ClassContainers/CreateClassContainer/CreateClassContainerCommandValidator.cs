using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.ClassContainers.CreateClassContainer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClassContainerCommandValidator : AbstractValidator<CreateClassContainerCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateClassContainerCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ClassPartitionKey)
                .NotNull();
        }
    }
}