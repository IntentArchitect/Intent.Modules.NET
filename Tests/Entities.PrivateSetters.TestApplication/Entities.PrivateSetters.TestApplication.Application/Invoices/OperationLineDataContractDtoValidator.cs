using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices
{
    public class OperationLineDataContractDtoValidator : AbstractValidator<OperationLineDataContractDto>
    {
        [IntentManaged(Mode.Merge)]
        public OperationLineDataContractDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}