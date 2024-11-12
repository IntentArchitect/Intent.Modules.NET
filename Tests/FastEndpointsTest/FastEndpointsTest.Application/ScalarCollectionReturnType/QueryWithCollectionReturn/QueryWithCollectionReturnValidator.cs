using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.ScalarCollectionReturnType.QueryWithCollectionReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryWithCollectionReturnValidator : AbstractValidator<QueryWithCollectionReturn>
    {
        [IntentManaged(Mode.Merge)]
        public QueryWithCollectionReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}