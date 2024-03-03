using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources.DeleteOneToOptionalSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOneToOptionalSourceCommandValidator : AbstractValidator<DeleteOneToOptionalSourceCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteOneToOptionalSourceCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}