using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.CreateClientByCtor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateClientByCtorCommandValidator : AbstractValidator<CreateClientByCtorCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateClientByCtorCommandValidator()
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