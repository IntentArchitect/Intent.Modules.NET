using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootCompositeManyBSQueryValidator : AbstractValidator<GetAggregateRootCompositeManyBSQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetAggregateRootCompositeManyBSQueryValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}