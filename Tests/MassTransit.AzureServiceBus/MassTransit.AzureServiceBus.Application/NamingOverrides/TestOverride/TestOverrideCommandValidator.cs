using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.NamingOverrides.TestOverride
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestOverrideCommandValidator : AbstractValidator<TestOverrideCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestOverrideCommandValidator()
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