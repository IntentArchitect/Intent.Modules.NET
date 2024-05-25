using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CompanyCreateDtoValidator : AbstractValidator<CompanyCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public CompanyCreateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.ContactDetailsVOS)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CompanyCreateCompanyContactDetailsVODto>()!));
        }
    }
}