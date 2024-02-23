using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.CreateClassContainer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClassContainerCommandValidator : AbstractValidator<CreateClassContainerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateClassContainerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ClassPartitionKey)
                .NotNull();
        }
    }
}