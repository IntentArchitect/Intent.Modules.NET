using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.Clients.CreateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Type)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}