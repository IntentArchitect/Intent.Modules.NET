using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace RichDomain.Application.People.UpdatePersonPerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePersonPersonCommandValidator : AbstractValidator<UpdatePersonPersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePersonPersonCommandValidator()
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