using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserWithTaskItemServiceSubTasksDtoValidator : AbstractValidator<CreateUserWithTaskItemServiceSubTasksDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserWithTaskItemServiceSubTasksDtoValidator()
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