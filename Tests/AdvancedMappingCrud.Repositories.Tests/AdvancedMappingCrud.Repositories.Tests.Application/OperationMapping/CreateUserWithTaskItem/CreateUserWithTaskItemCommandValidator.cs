using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping.CreateUserWithTaskItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserWithTaskItemCommandValidator : AbstractValidator<CreateUserWithTaskItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserWithTaskItemCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.UserName)
                .NotNull();

            RuleFor(v => v.ListName)
                .NotNull();

            RuleFor(v => v.TaskName)
                .NotNull();

            RuleFor(v => v.SubTasks)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateUserWithTaskItemCommandSubTasksDto>()!));
        }
    }
}