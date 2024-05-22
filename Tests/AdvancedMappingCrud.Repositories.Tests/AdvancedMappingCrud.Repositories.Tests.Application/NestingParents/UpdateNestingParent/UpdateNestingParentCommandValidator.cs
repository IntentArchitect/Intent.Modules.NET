using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.UpdateNestingParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateNestingParentCommandValidator : AbstractValidator<UpdateNestingParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateNestingParentCommandValidator()
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