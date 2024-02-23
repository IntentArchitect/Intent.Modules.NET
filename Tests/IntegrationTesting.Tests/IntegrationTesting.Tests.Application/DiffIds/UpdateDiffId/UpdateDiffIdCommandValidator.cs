using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.UpdateDiffId
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDiffIdCommandValidator : AbstractValidator<UpdateDiffIdCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDiffIdCommandValidator()
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