using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DBAssigneds.CreateDBAssigned
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDBAssignedCommandValidator : AbstractValidator<CreateDBAssignedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDBAssignedCommandValidator()
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