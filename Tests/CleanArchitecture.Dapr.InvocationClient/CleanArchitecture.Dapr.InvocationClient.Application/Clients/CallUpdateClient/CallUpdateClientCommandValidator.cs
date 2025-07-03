using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallUpdateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallUpdateClientCommandValidator : AbstractValidator<CallUpdateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallUpdateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.TagsIds)
                .NotNull();
        }
    }
}