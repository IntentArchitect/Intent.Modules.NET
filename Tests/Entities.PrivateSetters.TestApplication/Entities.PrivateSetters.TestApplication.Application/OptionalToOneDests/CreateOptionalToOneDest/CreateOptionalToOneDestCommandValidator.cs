using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.CreateOptionalToOneDest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOptionalToOneDestCommandValidator : AbstractValidator<CreateOptionalToOneDestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToOneDestCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}