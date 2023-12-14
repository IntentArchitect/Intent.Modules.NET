using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.NonStringPartitionKeys.GetNonStringPartitionKeyById
{
    public class GetNonStringPartitionKeyByIdQueryValidator : AbstractValidator<GetNonStringPartitionKeyByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNonStringPartitionKeyByIdQueryValidator()
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