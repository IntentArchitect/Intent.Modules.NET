using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.UpdateClassContainer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateClassContainerCommandValidator : AbstractValidator<UpdateClassContainerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClassContainerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.ClassPartitionKey)
                .NotNull();
        }
    }
}