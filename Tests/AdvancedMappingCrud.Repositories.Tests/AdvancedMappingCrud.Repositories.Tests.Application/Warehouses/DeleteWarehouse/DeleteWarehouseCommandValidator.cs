using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.DeleteWarehouse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteWarehouseCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}