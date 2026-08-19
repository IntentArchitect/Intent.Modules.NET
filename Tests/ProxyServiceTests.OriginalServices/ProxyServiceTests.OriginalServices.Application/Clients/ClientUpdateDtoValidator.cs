using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Clients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public ClientUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}