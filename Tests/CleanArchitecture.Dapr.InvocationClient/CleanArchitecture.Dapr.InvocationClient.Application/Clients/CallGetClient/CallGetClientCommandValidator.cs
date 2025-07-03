using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallGetClientCommandValidator : AbstractValidator<CallGetClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallGetClientCommandValidator()
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