using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomerByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerByNameQueryValidator : AbstractValidator<GetCustomerByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerByNameQueryValidator()
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