using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.CreateDerivedType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDerivedTypeCommandValidator : AbstractValidator<CreateDerivedTypeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDerivedTypeCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.DerivedName)
                .NotNull();

            RuleFor(v => v.BaseName)
                .NotNull();

            RuleFor(v => v.DerivedTypeAggregate)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateDerivedTypeDerivedTypeAggregateDto>()!);
        }
    }
}