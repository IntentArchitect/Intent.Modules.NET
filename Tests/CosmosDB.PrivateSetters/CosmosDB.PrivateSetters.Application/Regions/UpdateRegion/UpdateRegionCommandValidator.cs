using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Regions.UpdateRegion
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateRegionCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}