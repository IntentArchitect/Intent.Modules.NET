using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.GetNonStringPartitionKeyById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNonStringPartitionKeyByIdQueryValidator : AbstractValidator<GetNonStringPartitionKeyByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNonStringPartitionKeyByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}