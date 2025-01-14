using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCNCCChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCNCCChildCommandValidator : AbstractValidator<CreateCNCCChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCNCCChildCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}