using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEmbeddedParentEmbeddedChildDtoValidator : AbstractValidator<UpdateEmbeddedParentEmbeddedChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEmbeddedParentEmbeddedChildDtoValidator()
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