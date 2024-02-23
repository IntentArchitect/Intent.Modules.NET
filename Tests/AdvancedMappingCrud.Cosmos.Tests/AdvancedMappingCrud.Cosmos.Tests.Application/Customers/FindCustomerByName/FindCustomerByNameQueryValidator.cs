using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.FindCustomerByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FindCustomerByNameQueryValidator : AbstractValidator<FindCustomerByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public FindCustomerByNameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}