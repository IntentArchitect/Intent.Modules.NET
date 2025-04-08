using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateParentCommandParentDetailsDtoValidator : AbstractValidator<CreateParentCommandParentDetailsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateParentCommandParentDetailsDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.DetailsLine1)
                .NotNull();

            RuleFor(v => v.DetailsLine2)
                .NotNull();

            RuleFor(v => v.ParentDetailsTags)
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateParentCommandParentDetailsTagsDto>()!));

            RuleFor(v => v.ParentSubDetails)
                .SetValidator(provider.GetValidator<CreateParentCommandParentSubDetailsDto>()!);
        }
    }
}