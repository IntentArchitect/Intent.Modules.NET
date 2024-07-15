using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRef
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderByRefQueryValidator : AbstractValidator<GetOrderByRefQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderByRefQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}