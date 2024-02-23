using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.CreateChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateChildCommandValidator : AbstractValidator<CreateChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateChildCommandValidator()
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