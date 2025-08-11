using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.UpdateNoteAccountNoResult
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNoteAccountNoResultCommandValidator : AbstractValidator<UpdateNoteAccountNoResultCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountNoResultCommandValidator()
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