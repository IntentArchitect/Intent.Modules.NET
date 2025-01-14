using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCNCCChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCNCCChildCommandValidator : AbstractValidator<UpdateCNCCChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCNCCChildCommandValidator()
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