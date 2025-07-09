using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.CreateManyToOneSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateManyToOneSourceCommandValidator : AbstractValidator<CreateManyToOneSourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateManyToOneSourceCommandValidator()
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