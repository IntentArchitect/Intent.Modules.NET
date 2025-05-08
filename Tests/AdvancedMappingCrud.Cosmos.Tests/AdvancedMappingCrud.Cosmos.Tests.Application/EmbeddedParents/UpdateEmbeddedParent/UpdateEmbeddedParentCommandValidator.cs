using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.UpdateEmbeddedParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateEmbeddedParentCommandValidator : AbstractValidator<UpdateEmbeddedParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateEmbeddedParentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Children)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<UpdateEmbeddedParentEmbeddedChildDto>()!));

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}