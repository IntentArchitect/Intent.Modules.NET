using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.DeleteNestingParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteNestingParentCommandValidator : AbstractValidator<DeleteNestingParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteNestingParentCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}