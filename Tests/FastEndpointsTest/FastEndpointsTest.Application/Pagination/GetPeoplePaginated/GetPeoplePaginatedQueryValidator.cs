using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Pagination.GetPeoplePaginated
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPeoplePaginatedQueryValidator : AbstractValidator<GetPeoplePaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPeoplePaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}