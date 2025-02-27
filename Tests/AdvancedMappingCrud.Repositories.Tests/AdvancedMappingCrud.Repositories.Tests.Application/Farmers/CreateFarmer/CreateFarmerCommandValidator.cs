using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateFarmer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateFarmerCommandValidator : AbstractValidator<CreateFarmerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateFarmerCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();
        }
    }
}