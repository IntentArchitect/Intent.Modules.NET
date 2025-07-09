using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests.CreateManyToManyDest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateManyToManyDestCommandValidator : AbstractValidator<CreateManyToManyDestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateManyToManyDestCommandValidator()
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