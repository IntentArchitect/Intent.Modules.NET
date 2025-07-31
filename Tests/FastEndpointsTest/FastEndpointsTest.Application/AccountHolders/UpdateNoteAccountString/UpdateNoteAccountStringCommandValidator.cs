using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.UpdateNoteAccountString
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNoteAccountStringCommandValidator : AbstractValidator<UpdateNoteAccountStringCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountStringCommandValidator()
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