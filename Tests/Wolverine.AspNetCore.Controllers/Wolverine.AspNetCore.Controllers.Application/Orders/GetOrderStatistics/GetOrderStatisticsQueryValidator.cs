using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.QueryValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Application.GetOrderStatistics
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderStatisticsQueryValidator : AbstractValidator<GetOrderStatisticsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderStatisticsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}