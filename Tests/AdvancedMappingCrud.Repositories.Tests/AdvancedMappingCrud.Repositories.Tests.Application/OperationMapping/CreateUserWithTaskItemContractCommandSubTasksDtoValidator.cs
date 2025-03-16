using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserWithTaskItemContractCommandSubTasksDtoValidator : AbstractValidator<CreateUserWithTaskItemContractCommandSubTasksDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserWithTaskItemContractCommandSubTasksDtoValidator()
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