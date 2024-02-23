using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.CreateParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateParentCommandValidator : AbstractValidator<CreateParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateParentCommandValidator()
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