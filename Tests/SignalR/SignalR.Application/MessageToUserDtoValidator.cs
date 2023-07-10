using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace SignalR.Application
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MessageToUserDtoValidator : AbstractValidator<MessageToUserDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public MessageToUserDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}