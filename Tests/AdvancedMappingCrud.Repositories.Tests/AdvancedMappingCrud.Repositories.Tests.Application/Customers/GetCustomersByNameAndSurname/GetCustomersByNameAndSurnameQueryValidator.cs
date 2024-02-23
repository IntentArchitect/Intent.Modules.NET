using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersByNameAndSurname
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersByNameAndSurnameQueryValidator : AbstractValidator<GetCustomersByNameAndSurnameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersByNameAndSurnameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();
        }
    }
}