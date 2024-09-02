using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByFirstNamePaginated
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPeopleByFirstNamePaginatedQueryValidator : AbstractValidator<GetPeopleByFirstNamePaginatedQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPeopleByFirstNamePaginatedQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull();
        }
    }
}