using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.UpdateNonStringPartitionKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNonStringPartitionKeyCommandValidator : AbstractValidator<UpdateNonStringPartitionKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNonStringPartitionKeyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}