using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.CommandValidator", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Application.New
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NewCommandValidator : AbstractValidator<NewCommand>
    {
        [IntentManaged(Mode.Merge)]
        public NewCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}