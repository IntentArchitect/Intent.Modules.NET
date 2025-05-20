using FluentValidation;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomerById;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.MyCustomers.GetMyCustomerById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMyCustomerByIdQueryValidator : AbstractValidator<GetMyCustomerByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMyCustomerByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}