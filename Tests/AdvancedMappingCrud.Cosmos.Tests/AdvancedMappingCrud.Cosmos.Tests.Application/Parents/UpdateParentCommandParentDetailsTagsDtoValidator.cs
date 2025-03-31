using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateParentCommandParentDetailsTagsDtoValidator : AbstractValidator<UpdateParentCommandParentDetailsTagsDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandParentDetailsTagsDtoValidator()
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