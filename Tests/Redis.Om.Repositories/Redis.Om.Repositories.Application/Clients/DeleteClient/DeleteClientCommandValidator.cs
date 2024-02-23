using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.DeleteClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}