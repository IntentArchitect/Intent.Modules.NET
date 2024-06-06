using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Brands.GetBrands
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBrandsQueryValidator : AbstractValidator<GetBrandsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBrandsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}