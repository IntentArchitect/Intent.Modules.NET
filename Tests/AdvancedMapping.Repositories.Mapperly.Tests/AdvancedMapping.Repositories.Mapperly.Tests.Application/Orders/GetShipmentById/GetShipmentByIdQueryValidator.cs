using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.GetShipmentById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetShipmentByIdQueryValidator : AbstractValidator<GetShipmentByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetShipmentByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}