using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.PagingTS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagingTSCreateDtoValidator : AbstractValidator<PagingTSCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public PagingTSCreateDtoValidator()
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