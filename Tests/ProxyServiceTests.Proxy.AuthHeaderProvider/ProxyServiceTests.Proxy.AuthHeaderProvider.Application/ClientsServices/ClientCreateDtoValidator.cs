using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.ClientsServices
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreateDtoValidator : AbstractValidator<ClientCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public ClientCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}