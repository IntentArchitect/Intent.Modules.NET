using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Scheduled.PublishDelayedNotification
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PublishDelayedNotificationCommandValidator : AbstractValidator<PublishDelayedNotificationCommand>
    {
        [IntentManaged(Mode.Merge)]
        public PublishDelayedNotificationCommandValidator()
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