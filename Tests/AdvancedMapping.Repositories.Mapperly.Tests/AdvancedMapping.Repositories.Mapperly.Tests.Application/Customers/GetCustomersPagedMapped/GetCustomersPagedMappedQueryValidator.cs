using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetCustomersPagedMapped
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersPagedMappedQueryValidator : AbstractValidator<GetCustomersPagedMappedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedMappedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}