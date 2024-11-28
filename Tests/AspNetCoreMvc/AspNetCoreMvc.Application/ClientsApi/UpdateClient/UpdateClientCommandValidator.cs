using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AspNetCoreMvc.Application.ClientsApi.UpdateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}