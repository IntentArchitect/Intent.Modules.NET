using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.CreateManyToOneDest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateManyToOneDestCommandValidator : AbstractValidator<CreateManyToOneDestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateManyToOneDestCommandValidator()
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