using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Companies.CreateCompany
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateCompanyCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.ContactDetailsVOS)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateCompanyContactDetailsVODto>()!));
        }
    }
}