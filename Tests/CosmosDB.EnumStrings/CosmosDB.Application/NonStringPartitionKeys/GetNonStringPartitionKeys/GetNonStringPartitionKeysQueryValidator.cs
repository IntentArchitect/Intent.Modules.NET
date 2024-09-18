using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.GetNonStringPartitionKeys
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetNonStringPartitionKeysQueryValidator : AbstractValidator<GetNonStringPartitionKeysQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetNonStringPartitionKeysQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}