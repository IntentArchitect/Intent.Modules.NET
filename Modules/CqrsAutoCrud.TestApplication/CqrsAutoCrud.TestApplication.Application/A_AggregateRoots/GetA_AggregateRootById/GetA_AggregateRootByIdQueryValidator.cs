using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.GetA_AggregateRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetA_AggregateRootByIdQueryValidator : AbstractValidator<GetA_AggregateRootByIdQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetA_AggregateRootByIdQueryValidator()
        {
        }
    }
}