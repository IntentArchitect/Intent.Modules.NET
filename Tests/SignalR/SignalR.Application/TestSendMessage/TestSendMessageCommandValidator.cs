using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace SignalR.Application.TestSendMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestSendMessageCommandValidator : AbstractValidator<TestSendMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestSendMessageCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}