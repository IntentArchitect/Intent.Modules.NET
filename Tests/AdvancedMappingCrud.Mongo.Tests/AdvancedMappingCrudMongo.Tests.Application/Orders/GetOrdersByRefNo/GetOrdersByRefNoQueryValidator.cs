using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrdersByRefNo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrdersByRefNoQueryValidator : AbstractValidator<GetOrdersByRefNoQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrdersByRefNoQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}