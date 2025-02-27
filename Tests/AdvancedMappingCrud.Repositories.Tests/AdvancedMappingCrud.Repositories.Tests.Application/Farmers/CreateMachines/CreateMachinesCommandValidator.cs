using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateMachines
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMachinesCommandValidator : AbstractValidator<CreateMachinesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMachinesCommandValidator()
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