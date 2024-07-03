using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersWithParams
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersWithParamsQueryValidator : AbstractValidator<GetCustomersWithParamsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersWithParamsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}