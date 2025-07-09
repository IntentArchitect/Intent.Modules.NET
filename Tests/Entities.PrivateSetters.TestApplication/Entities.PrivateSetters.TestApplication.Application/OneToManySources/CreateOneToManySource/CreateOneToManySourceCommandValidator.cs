using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.CreateOneToManySource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneToManySourceCommandValidator : AbstractValidator<CreateOneToManySourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneToManySourceCommandValidator()
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