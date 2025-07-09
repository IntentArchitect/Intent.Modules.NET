using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.CreateOptionalToManyDest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOptionalToManyDestCommandValidator : AbstractValidator<CreateOptionalToManyDestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOptionalToManyDestCommandValidator()
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