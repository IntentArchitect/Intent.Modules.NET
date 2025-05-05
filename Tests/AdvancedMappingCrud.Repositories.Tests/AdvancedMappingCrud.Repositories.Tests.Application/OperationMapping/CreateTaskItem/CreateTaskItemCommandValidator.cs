using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateTaskItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateTaskItemCommandValidator()
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