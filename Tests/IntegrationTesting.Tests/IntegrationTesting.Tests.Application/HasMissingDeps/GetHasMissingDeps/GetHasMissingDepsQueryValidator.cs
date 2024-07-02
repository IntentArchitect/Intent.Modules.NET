using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.GetHasMissingDeps
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetHasMissingDepsQueryValidator : AbstractValidator<GetHasMissingDepsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetHasMissingDepsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}