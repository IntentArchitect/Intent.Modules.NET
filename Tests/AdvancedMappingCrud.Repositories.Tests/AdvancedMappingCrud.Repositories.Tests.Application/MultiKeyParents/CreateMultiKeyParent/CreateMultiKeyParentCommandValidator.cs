using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.CreateMultiKeyParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMultiKeyParentCommandValidator : AbstractValidator<CreateMultiKeyParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMultiKeyParentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.MultiKeyChildren)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateMultiKeyParentCommandMultiKeyChildrenDto>()!));
        }
    }
}