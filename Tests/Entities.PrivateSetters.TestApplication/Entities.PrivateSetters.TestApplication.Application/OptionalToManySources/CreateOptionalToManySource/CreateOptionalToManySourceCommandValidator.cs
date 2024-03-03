using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManySources.CreateOptionalToManySource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOptionalToManySourceCommandValidator : AbstractValidator<CreateOptionalToManySourceCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public CreateOptionalToManySourceCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}