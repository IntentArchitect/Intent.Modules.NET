using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Application.Customers.GetCustomersWithStatus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersWithStatusQueryValidator : AbstractValidator<GetCustomersWithStatusQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersWithStatusQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}