using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.Validators
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrgCommandDepartmentsDtoValidator : AbstractValidator<CreateOrgCommandDepartmentsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrgCommandDepartmentsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Code)
                .NotNull();

            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}