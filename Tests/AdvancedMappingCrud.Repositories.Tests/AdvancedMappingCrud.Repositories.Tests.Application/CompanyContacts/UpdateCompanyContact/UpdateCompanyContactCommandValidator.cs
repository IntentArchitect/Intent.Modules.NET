using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.UpdateCompanyContact
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateCompanyContactCommandValidator : AbstractValidator<UpdateCompanyContactCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCompanyContactCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.FirstName)
                .NotNull();
        }
    }
}