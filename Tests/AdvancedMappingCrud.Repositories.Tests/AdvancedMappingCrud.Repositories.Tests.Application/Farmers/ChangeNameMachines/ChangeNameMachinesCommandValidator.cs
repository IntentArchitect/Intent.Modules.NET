using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.ChangeNameMachines
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeNameMachinesCommandValidator : AbstractValidator<ChangeNameMachinesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeNameMachinesCommandValidator()
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