using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Regions.GetRegionsByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRegionsByNameQueryValidator : AbstractValidator<GetRegionsByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRegionsByNameQueryValidator()
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