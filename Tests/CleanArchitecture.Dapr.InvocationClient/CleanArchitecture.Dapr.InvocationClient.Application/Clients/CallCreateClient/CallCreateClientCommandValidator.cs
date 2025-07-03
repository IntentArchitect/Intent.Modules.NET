using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallCreateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallCreateClientCommandValidator : AbstractValidator<CallCreateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallCreateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TagsIds)
                .NotNull();
        }
    }
}