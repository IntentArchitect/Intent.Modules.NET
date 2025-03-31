using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.UpdateParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateParentCommandValidator : AbstractValidator<UpdateParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.Children)
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateParentCommandChildrenDto>()!));

            RuleFor(v => v.ParentDetails)
                .SetValidator(provider.GetValidator<UpdateParentCommandParentDetailsDto>()!);
        }
    }
}