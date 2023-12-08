using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.GetOrderById
{
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}