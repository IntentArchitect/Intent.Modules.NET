using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.Pagination.GetPeopleByNullableFirstNamePaginated
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPeopleByNullableFirstNamePaginatedQueryValidator : AbstractValidator<GetPeopleByNullableFirstNamePaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPeopleByNullableFirstNamePaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}