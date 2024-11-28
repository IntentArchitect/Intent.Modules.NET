using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCoreMvc.Application.ClientsApi.CreateClient
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
            // Implement custom validation logic here if required
        }
    }
}