using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.DeleteNonStringPartitionKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteNonStringPartitionKeyCommandValidator : AbstractValidator<DeleteNonStringPartitionKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteNonStringPartitionKeyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}