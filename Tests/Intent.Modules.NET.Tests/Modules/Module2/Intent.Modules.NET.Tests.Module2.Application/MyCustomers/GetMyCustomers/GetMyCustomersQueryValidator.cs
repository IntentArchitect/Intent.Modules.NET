using FluentValidation;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.MyCustomers.GetMyCustomers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMyCustomersQueryValidator : AbstractValidator<GetMyCustomersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMyCustomersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}