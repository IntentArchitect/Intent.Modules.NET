using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Regions.GetRegionById
{
    public class GetRegionByIdQueryValidator : AbstractValidator<GetRegionByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRegionByIdQueryValidator()
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