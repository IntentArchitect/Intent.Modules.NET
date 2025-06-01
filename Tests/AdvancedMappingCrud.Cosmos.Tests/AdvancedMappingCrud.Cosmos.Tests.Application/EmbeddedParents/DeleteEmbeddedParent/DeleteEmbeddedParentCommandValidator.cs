using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.DeleteEmbeddedParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteEmbeddedParentCommandValidator : AbstractValidator<DeleteEmbeddedParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteEmbeddedParentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}