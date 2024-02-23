using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.SecuredProxy.GetSecuredValue
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetSecuredValueQueryValidator : AbstractValidator<GetSecuredValueQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetSecuredValueQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}