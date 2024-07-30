using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersPaginatedWithOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersPaginatedWithOrderQueryValidator : AbstractValidator<GetCustomersPaginatedWithOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersPaginatedWithOrderQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.OrderBy)
                .NotNull();
        }
    }
}