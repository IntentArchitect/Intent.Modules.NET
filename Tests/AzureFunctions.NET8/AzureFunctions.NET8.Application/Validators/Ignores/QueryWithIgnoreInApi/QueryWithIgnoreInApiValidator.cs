using AzureFunctions.NET8.Application.Ignores.QueryWithIgnoreInApi;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AzureFunctions.NET8.Application.Validators.Ignores.QueryWithIgnoreInApi
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryWithIgnoreInApiValidator : AbstractValidator<Application.Ignores.QueryWithIgnoreInApi.QueryWithIgnoreInApi>
    {
        [IntentManaged(Mode.Merge)]
        public QueryWithIgnoreInApiValidator()
        {
        }
    }
}