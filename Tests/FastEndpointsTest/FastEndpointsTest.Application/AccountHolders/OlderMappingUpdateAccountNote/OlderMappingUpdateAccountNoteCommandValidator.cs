using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.OlderMappingUpdateAccountNote
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OlderMappingUpdateAccountNoteCommandValidator : AbstractValidator<OlderMappingUpdateAccountNoteCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OlderMappingUpdateAccountNoteCommandValidator()
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