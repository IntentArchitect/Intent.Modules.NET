using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetAggregateRootByIdQueryValidator : AbstractValidator<GetAggregateRootByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetAggregateRootByIdQueryValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}