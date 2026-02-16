using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClientExtraFields
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CallGetClientExtraFieldsCommandValidator : AbstractValidator<CallGetClientExtraFieldsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CallGetClientExtraFieldsCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field1)
                .NotNull();

            RuleFor(v => v.Field2)
                .NotNull();
        }
    }
}