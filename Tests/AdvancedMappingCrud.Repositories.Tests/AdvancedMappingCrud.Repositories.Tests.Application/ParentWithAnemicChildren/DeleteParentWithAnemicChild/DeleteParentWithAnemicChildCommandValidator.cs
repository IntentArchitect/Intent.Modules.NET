using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.DeleteParentWithAnemicChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteParentWithAnemicChildCommandValidator : AbstractValidator<DeleteParentWithAnemicChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteParentWithAnemicChildCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}