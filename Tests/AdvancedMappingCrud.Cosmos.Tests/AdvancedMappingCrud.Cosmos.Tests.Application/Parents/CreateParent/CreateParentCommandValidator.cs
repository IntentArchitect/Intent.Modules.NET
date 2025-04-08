using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.CreateParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateParentCommandValidator : AbstractValidator<CreateParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateParentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Children)
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateParentCommandChildrenDto>()!));

            RuleFor(v => v.ParentDetails)
                .SetValidator(provider.GetValidator<CreateParentCommandParentDetailsDto>()!);
        }
    }
}