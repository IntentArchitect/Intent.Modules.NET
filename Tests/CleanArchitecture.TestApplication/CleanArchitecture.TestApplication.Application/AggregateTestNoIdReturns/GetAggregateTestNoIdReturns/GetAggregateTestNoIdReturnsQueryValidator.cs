using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturns
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateTestNoIdReturnsQueryValidator : AbstractValidator<GetAggregateTestNoIdReturnsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public GetAggregateTestNoIdReturnsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}