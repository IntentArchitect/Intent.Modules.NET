using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.DeleteExternalDoc
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteExternalDocCommandValidator : AbstractValidator<DeleteExternalDocCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteExternalDocCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}