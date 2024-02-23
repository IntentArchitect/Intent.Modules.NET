using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.DerivedOfTS.GetDerivedOfTById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDerivedOfTByIdQueryValidator : AbstractValidator<GetDerivedOfTByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDerivedOfTByIdQueryValidator()
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