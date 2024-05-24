using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDtoValidator : AbstractValidator<UpdateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Details)
                .NotNull()
                .SetValidator(provider.GetValidator<UpdateUpdatePersonDetailsDto>()!);
        }
    }
}