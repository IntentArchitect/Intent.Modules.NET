using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ParamConversion.CheckTypeConversionsOnProxy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CheckTypeConversionsOnProxyValidator : AbstractValidator<CheckTypeConversionsOnProxy>
    {
        [IntentManaged(Mode.Merge)]
        public CheckTypeConversionsOnProxyValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}