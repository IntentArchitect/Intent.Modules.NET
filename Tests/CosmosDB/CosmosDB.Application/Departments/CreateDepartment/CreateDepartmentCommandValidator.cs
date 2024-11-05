using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.Application.Departments.CreateDepartment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDepartmentCommandValidator()
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