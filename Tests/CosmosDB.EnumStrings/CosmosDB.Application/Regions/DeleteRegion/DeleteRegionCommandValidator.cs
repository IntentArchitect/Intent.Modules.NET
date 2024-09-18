using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.Regions.DeleteRegion
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteRegionCommandValidator : AbstractValidator<DeleteRegionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteRegionCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}