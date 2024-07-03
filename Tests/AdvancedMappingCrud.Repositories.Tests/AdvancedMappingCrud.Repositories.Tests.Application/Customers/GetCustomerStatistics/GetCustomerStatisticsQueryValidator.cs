using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomerStatistics
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerStatisticsQueryValidator : AbstractValidator<GetCustomerStatisticsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerStatisticsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}