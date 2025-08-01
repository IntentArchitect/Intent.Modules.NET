using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOneSources.CreateOneToOneSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneToOneSourceCommandValidator : AbstractValidator<CreateOneToOneSourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneToOneSourceCommandValidator()
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