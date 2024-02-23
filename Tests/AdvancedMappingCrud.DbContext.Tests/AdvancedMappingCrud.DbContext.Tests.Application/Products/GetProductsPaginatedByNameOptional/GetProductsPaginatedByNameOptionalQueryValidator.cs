using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByNameOptional
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPaginatedByNameOptionalQueryValidator : AbstractValidator<GetProductsPaginatedByNameOptionalQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameOptionalQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}