using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.GetShipments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetShipmentsQueryValidator : AbstractValidator<GetShipmentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetShipmentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}