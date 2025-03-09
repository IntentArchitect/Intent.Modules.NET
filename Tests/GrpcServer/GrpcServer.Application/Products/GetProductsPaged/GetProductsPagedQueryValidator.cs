using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace GrpcServer.Application.Products.GetProductsPaged
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetProductsPagedQueryValidator : AbstractValidator<GetProductsPagedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsPagedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}