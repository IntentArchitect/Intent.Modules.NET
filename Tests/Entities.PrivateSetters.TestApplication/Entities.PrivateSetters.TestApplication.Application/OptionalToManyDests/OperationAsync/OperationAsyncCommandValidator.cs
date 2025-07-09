using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests.OperationAsync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OperationAsyncCommandValidator : AbstractValidator<OperationAsyncCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OperationAsyncCommandValidator()
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