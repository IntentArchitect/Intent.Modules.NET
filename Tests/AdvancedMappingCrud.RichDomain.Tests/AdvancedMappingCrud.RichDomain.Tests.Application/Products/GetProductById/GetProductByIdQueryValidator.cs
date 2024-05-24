using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.GetProductById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}