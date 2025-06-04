using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerStatistics
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBuyerStatisticsQueryValidator : AbstractValidator<GetBuyerStatisticsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBuyerStatisticsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}