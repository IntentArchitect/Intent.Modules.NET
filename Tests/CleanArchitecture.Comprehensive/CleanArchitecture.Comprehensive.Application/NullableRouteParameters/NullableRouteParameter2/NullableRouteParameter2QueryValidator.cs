using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter2
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NullableRouteParameter2QueryValidator : AbstractValidator<NullableRouteParameter2Query>
    {
        [IntentManaged(Mode.Merge)]
        public NullableRouteParameter2QueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.NullableEnum)
                .IsInEnum();
        }
    }
}