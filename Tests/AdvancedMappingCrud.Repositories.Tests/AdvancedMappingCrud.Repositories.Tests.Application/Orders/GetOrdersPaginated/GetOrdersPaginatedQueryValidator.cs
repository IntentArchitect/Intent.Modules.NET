using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.GetOrdersPaginated
{
    public class GetOrdersPaginatedQueryValidator : AbstractValidator<GetOrdersPaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrdersPaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}