using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.CreateOptionalToOneSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOptionalToOneSourceCommandValidator : AbstractValidator<CreateOptionalToOneSourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToOneSourceCommandValidator()
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