using AzureFunctions.TestApplication.Application.Ignores.QueryWithIgnoreInApi;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.Validators.Ignores.QueryWithIgnoreInApi
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryWithIgnoreInApiValidator : AbstractValidator<Application.Ignores.QueryWithIgnoreInApi.QueryWithIgnoreInApi>
    {
        [IntentManaged(Mode.Merge)]
        public QueryWithIgnoreInApiValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}