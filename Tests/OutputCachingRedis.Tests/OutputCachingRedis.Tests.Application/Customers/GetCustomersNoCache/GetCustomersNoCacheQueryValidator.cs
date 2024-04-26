using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Customers.GetCustomersNoCache
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersNoCacheQueryValidator : AbstractValidator<GetCustomersNoCacheQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersNoCacheQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SearchTerm)
                .NotNull();
        }
    }
}