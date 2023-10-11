using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.PaginationForProxies.PaginatedResult
{
    public class PaginatedResultQueryValidator : AbstractValidator<PaginatedResultQuery>
    {
        [IntentManaged(Mode.Merge)]
        public PaginatedResultQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}