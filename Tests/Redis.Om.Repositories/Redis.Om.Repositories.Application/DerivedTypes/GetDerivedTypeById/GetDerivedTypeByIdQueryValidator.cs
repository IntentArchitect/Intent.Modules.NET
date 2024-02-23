using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypeById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDerivedTypeByIdQueryValidator : AbstractValidator<GetDerivedTypeByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDerivedTypeByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}