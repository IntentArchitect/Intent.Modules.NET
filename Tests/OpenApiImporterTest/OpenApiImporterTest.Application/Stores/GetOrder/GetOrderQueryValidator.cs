using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.GetOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}