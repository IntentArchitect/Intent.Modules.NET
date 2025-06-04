using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetBuyersQueryValidator : AbstractValidator<GetBuyersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetBuyersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}