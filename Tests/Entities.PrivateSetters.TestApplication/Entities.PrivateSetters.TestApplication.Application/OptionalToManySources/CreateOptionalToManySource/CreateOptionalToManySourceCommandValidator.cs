using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.CreateOptionalToManySource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOptionalToManySourceCommandValidator : AbstractValidator<CreateOptionalToManySourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToManySourceCommandValidator()
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