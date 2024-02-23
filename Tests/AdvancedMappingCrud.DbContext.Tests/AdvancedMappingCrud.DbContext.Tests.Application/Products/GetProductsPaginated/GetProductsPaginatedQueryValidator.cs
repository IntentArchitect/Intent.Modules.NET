using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginated
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPaginatedQueryValidator : AbstractValidator<GetProductsPaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}