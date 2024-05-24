using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddCollectionsDtoValidator : AbstractValidator<AddCollectionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public AddCollectionsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Addresses)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<AddCollectionsAddAddressDCDto>()!));

            RuleFor(v => v.Contacts)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<AddCollectionsAddContactDetailsVODto>()!));
        }
    }
}