using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallGetClientsCommandValidator : AbstractValidator<CallGetClientsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallGetClientsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}