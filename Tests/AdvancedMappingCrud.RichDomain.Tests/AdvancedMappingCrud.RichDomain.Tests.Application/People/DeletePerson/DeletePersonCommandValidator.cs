using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People.DeletePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeletePersonCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}