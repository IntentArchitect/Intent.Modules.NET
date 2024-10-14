using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.CreateParentWithAnemicChild
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateParentWithAnemicChildCommandValidator : AbstractValidator<CreateParentWithAnemicChildCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateParentWithAnemicChildCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Surname)
                .NotNull();

            RuleFor(v => v.AnemicChildren)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateParentWithAnemicChildAnemicChildDto>()!));
        }
    }
}