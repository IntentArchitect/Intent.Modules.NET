using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones.CreateOne
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneCommandValidator : AbstractValidator<CreateOneCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.OneName)
                .NotNull();

            RuleFor(v => v.Two)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateOneTwoDto>()!);
        }
    }
}