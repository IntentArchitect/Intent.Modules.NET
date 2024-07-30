using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.PagingTS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagingTSUpdateDtoValidator : AbstractValidator<PagingTSUpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public PagingTSUpdateDtoValidator()
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