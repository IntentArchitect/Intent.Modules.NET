using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Regions.GetRegionById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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