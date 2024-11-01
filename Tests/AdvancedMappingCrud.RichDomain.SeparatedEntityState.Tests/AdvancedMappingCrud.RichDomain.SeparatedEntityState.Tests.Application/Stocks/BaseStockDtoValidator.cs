using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BaseStockDtoValidator : AbstractValidator<BaseStockDto>
    {
        [IntentManaged(Mode.Merge)]
        public BaseStockDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.User)
                .NotNull();
        }
    }
}