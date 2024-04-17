using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.CreateStoreOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateStoreOrderCommandValidator : AbstractValidator<CreateStoreOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateStoreOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ShipDate)
                .NotNull();

            RuleFor(v => v.Status)
                .NotNull()
                .IsInEnum();
        }
    }
}