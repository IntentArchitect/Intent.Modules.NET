using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.CreateOneToOptionalSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneToOptionalSourceCommandValidator : AbstractValidator<CreateOneToOptionalSourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneToOptionalSourceCommandValidator()
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