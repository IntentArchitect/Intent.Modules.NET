using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.GetAggregateRootAById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootAByIdQueryValidator : AbstractValidator<GetAggregateRootAByIdQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetAggregateRootAByIdQueryValidator()
        {
        }
    }
}