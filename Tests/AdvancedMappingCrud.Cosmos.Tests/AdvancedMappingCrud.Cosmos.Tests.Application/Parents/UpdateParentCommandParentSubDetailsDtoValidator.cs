using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateParentCommandParentSubDetailsDtoValidator : AbstractValidator<UpdateParentCommandParentSubDetailsDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandParentSubDetailsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.SubDetailsLine1)
                .NotNull();

            RuleFor(v => v.SubDetailsLine2)
                .NotNull();
        }
    }
}