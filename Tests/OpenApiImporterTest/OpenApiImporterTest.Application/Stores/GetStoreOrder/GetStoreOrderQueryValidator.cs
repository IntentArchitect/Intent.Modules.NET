using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.GetStoreOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetStoreOrderQueryValidator : AbstractValidator<GetStoreOrderQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetStoreOrderQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}