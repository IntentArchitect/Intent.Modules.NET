using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.GetWarehouses
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetWarehousesQueryValidator : AbstractValidator<GetWarehousesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetWarehousesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}