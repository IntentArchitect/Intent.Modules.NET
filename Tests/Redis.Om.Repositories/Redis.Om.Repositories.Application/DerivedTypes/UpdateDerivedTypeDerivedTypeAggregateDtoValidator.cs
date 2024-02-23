using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDerivedTypeDerivedTypeAggregateDtoValidator : AbstractValidator<UpdateDerivedTypeDerivedTypeAggregateDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDerivedTypeDerivedTypeAggregateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AggregateName)
                .NotNull();
        }
    }
}