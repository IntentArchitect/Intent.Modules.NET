using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.GetInventory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInventoryQueryValidator : AbstractValidator<GetInventoryQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInventoryQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}