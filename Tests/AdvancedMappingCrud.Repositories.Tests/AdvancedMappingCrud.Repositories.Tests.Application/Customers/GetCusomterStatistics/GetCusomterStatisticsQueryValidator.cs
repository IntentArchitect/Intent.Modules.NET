using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCusomterStatistics
{
    public class GetCusomterStatisticsQueryValidator : AbstractValidator<GetCusomterStatisticsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCusomterStatisticsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}