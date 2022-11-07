using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots.GetAggregateRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootByIdQueryValidator : AbstractValidator<GetAggregateRootByIdQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetAggregateRootByIdQueryValidator()
        {
        }
    }
}