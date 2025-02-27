using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddPlotDtoValidator : AbstractValidator<AddPlotDto>
    {
        [IntentManaged(Mode.Merge)]
        public AddPlotDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();
        }
    }
}