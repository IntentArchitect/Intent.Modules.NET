using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRefNo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderByRefNoQueryValidator : AbstractValidator<GetOrderByRefNoQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderByRefNoQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();

            RuleFor(v => v.External)
                .NotNull();
        }
    }
}