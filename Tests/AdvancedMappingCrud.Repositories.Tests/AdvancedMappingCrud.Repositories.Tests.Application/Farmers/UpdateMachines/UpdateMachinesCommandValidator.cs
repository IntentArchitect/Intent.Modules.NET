using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateMachines
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateMachinesCommandValidator : AbstractValidator<UpdateMachinesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateMachinesCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}