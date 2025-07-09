using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManySource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationOneToManySourceCommandValidator : AbstractValidator<OperationOneToManySourceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OperationOneToManySourceCommandValidator()
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