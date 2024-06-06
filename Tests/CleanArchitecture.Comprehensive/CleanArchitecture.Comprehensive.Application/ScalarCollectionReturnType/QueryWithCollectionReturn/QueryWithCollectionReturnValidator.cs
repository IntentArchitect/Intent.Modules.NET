using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ScalarCollectionReturnType.QueryWithCollectionReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryWithCollectionReturnValidator : AbstractValidator<QueryWithCollectionReturn>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public QueryWithCollectionReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}