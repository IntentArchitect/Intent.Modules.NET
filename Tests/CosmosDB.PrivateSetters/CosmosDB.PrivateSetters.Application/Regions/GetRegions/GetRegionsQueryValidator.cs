using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Regions.GetRegions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRegionsQueryValidator : AbstractValidator<GetRegionsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRegionsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}