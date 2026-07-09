using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Application.ChangeProductProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeProductProductCommandValidator : AbstractValidator<ChangeProductProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeProductProductCommandValidator()
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