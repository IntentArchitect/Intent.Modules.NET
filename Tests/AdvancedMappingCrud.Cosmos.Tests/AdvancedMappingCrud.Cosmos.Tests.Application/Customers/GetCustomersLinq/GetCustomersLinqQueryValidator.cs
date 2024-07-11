using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomersLinq
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersLinqQueryValidator : AbstractValidator<GetCustomersLinqQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersLinqQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}