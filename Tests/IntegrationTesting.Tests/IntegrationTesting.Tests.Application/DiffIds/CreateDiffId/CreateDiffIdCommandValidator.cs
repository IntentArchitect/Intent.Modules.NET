using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.CreateDiffId
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDiffIdCommandValidator : AbstractValidator<CreateDiffIdCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDiffIdCommandValidator()
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