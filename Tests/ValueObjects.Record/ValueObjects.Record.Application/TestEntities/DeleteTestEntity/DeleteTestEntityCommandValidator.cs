using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ValueObjects.Record.Application.TestEntities.DeleteTestEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteTestEntityCommandValidator : AbstractValidator<DeleteTestEntityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteTestEntityCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}