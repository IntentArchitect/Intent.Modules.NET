using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        [IntentManaged(Mode.Merge)]
        public ClientDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Identifier)
                .NotNull();

            RuleFor(v => v.Type)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}