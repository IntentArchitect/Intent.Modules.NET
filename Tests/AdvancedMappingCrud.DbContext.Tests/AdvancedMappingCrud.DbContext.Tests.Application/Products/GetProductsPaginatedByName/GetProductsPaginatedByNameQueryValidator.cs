using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPaginatedByNameQueryValidator : AbstractValidator<GetProductsPaginatedByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameQueryValidator()
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