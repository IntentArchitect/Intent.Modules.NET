using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Containers.CreateVessel
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateVesselCommandValidator : AbstractValidator<CreateVesselCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateVesselCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.IMOCode)
                .NotNull();
        }
    }
}