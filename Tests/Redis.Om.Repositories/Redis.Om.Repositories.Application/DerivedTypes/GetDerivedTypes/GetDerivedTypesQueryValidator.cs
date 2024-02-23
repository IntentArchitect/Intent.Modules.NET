using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDerivedTypesQueryValidator : AbstractValidator<GetDerivedTypesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDerivedTypesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}