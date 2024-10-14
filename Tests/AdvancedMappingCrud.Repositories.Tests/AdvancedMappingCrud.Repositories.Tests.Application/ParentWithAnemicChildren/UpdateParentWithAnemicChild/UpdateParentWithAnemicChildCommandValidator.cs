using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.UpdateParentWithAnemicChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateParentWithAnemicChildCommandValidator : AbstractValidator<UpdateParentWithAnemicChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateParentWithAnemicChildCommandValidator()
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