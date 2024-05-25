using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddCollectionsAddAddressDCDtoValidator : AbstractValidator<AddCollectionsAddAddressDCDto>
    {
        [IntentManaged(Mode.Merge)]
        public AddCollectionsAddAddressDCDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Line1)
                .NotNull();

            RuleFor(v => v.Line2)
                .NotNull();

            RuleFor(v => v.City)
                .NotNull();
        }
    }
}