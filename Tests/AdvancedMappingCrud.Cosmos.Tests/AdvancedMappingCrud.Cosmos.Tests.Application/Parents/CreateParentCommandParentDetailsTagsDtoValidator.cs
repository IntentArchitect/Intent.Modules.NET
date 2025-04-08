using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateParentCommandParentDetailsTagsDtoValidator : AbstractValidator<CreateParentCommandParentDetailsTagsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateParentCommandParentDetailsTagsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.TagName)
                .NotNull();

            RuleFor(v => v.TagValue)
                .NotNull();
        }
    }
}