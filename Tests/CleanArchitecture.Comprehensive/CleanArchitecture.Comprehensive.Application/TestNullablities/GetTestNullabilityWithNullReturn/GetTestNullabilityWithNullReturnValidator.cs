using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullabilityWithNullReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetTestNullabilityWithNullReturnValidator : AbstractValidator<GetTestNullabilityWithNullReturn>
    {
        [IntentManaged(Mode.Merge)]
        public GetTestNullabilityWithNullReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}