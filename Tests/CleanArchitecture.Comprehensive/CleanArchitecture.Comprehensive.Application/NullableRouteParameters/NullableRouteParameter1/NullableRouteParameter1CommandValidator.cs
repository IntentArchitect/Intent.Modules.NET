using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NullableRouteParameter1CommandValidator : AbstractValidator<NullableRouteParameter1Command>
    {
        [IntentManaged(Mode.Merge)]
        public NullableRouteParameter1CommandValidator()
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