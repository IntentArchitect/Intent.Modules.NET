using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DBAssigneds.UpdateDBAssigned
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDBAssignedCommandValidator : AbstractValidator<UpdateDBAssignedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDBAssignedCommandValidator()
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