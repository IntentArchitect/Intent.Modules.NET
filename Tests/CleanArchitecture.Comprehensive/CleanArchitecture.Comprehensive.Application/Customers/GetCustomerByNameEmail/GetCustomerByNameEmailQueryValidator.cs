using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.GetCustomerByNameEmail
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerByNameEmailQueryValidator : AbstractValidator<GetCustomerByNameEmailQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerByNameEmailQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}