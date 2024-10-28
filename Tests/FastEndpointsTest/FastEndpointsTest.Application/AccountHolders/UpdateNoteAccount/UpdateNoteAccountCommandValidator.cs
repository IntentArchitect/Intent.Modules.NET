using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.UpdateNoteAccount
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNoteAccountCommandValidator : AbstractValidator<UpdateNoteAccountCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Note)
                .NotNull();
        }
    }
}