using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.UpdateParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateParentCommandValidator : AbstractValidator<UpdateParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandValidator()
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