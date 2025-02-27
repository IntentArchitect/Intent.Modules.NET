using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.ChangeNameFarmer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeNameFarmerCommandValidator : AbstractValidator<ChangeNameFarmerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeNameFarmerCommandValidator()
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