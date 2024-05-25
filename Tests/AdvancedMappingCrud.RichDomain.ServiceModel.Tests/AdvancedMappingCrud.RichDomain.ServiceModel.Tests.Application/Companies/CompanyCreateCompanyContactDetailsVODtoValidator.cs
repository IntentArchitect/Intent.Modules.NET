using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompanyCreateCompanyContactDetailsVODtoValidator : AbstractValidator<CompanyCreateCompanyContactDetailsVODto>
    {
        [IntentManaged(Mode.Merge)]
        public CompanyCreateCompanyContactDetailsVODtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Cell)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();
        }
    }
}