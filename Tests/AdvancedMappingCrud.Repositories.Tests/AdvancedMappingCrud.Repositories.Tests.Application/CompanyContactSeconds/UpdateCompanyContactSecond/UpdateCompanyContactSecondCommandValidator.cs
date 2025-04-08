using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.UpdateCompanyContactSecond
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCompanyContactSecondCommandValidator : AbstractValidator<UpdateCompanyContactSecondCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCompanyContactSecondCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ContactName)
                .NotNull();

            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}