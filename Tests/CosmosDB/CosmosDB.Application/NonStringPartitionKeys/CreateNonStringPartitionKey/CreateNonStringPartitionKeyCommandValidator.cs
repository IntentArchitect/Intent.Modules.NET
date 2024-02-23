using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.CreateNonStringPartitionKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNonStringPartitionKeyCommandValidator : AbstractValidator<CreateNonStringPartitionKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNonStringPartitionKeyCommandValidator()
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