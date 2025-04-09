using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.GetPersonByName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPersonByNameQueryValidator : AbstractValidator<GetPersonByNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPersonByNameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}