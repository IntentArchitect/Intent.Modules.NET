using AzureFunctions.AzureServiceBus.Application.Common.Validation;
using AzureFunctions.AzureServiceBus.Application.Org.CreateOrg;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.Validators.Org.CreateOrg
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrgCommandValidator : AbstractValidator<CreateOrgCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrgCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Type)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Departments)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOrgCommandDepartmentsDto>()!));
        }
    }
}