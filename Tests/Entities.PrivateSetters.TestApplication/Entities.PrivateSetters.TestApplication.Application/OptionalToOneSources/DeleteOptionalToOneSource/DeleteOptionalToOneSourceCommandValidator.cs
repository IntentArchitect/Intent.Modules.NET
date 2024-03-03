using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources.DeleteOptionalToOneSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOptionalToOneSourceCommandValidator : AbstractValidator<DeleteOptionalToOneSourceCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteOptionalToOneSourceCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}