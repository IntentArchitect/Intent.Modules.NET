using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.DeleteOneToManySourceOneToManyDest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOneToManySourceOneToManyDestCommandValidator : AbstractValidator<DeleteOneToManySourceOneToManyDestCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteOneToManySourceOneToManyDestCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}