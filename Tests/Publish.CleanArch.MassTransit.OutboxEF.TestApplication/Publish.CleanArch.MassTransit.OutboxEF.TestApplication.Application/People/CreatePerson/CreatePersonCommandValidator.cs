using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.People.CreatePerson
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePersonCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull();

            RuleFor(v => v.LastName)
                .NotNull();
        }
    }
}