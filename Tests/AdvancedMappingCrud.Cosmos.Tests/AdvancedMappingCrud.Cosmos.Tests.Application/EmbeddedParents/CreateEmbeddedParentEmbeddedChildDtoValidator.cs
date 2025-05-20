using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateEmbeddedParentEmbeddedChildDtoValidator : AbstractValidator<CreateEmbeddedParentEmbeddedChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateEmbeddedParentEmbeddedChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}