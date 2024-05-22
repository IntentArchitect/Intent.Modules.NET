using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.CreateNestingParent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNestingParentCommandValidator : AbstractValidator<CreateNestingParentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNestingParentCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.NestingChildren)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateNestingParentCommandNestingChildrenDto>()!));
        }
    }
}