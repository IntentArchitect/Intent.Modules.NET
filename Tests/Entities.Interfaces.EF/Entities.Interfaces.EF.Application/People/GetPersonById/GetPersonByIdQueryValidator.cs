using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.People.GetPersonById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPersonByIdQueryValidator : AbstractValidator<GetPersonByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPersonByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}