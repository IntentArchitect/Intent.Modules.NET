using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.People.UpdatePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePersonCommandValidator()
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