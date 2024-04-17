using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.GetStoreInventory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetStoreInventoryQueryValidator : AbstractValidator<GetStoreInventoryQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetStoreInventoryQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}