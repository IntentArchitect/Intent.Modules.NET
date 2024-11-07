using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StockDtoValidator : AbstractValidator<StockDto>
    {
        [IntentManaged(Mode.Merge)]
        public StockDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            Include(provider.GetValidator<BaseStockDto>());

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}