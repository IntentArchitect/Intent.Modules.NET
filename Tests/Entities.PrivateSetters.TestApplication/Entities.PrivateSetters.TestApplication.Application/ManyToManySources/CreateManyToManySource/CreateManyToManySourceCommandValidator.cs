using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources.CreateManyToManySource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateManyToManySourceCommandValidator : AbstractValidator<CreateManyToManySourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateManyToManySourceCommandValidator()
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