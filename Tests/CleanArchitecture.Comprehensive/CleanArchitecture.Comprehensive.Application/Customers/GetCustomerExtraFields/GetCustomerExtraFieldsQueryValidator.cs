using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.GetCustomerExtraFields
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerExtraFieldsQueryValidator : AbstractValidator<GetCustomerExtraFieldsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerExtraFieldsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field1)
                .NotNull();

            RuleFor(v => v.Field2)
                .NotNull();
        }
    }
}