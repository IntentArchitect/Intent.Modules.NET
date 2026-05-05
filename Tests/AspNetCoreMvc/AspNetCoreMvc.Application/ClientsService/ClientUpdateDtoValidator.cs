using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AspNetCoreMvc.Application.ClientsService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public ClientUpdateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}