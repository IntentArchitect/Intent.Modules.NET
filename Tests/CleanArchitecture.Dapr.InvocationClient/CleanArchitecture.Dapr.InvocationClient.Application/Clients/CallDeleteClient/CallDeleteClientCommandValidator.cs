using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallDeleteClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallDeleteClientCommandValidator : AbstractValidator<CallDeleteClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallDeleteClientCommandValidator()
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