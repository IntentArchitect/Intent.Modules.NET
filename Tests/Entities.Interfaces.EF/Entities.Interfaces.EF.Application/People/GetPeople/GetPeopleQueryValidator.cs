using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.People.GetPeople
{
    public class GetPeopleQueryValidator : AbstractValidator<GetPeopleQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPeopleQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}