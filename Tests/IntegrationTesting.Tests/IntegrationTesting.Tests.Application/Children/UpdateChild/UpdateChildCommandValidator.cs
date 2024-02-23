using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.UpdateChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateChildCommandValidator : AbstractValidator<UpdateChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateChildCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}