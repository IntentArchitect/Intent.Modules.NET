using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandDispatchDocumentDtoValidator : AbstractValidator<CreateOrderCommandDispatchDocumentDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandDispatchDocumentDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DocumentNumber)
                .NotNull();
        }
    }
}