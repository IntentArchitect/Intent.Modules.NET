using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.GetA_AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetA_AggregateRootsQueryValidator : AbstractValidator<GetA_AggregateRootsQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetA_AggregateRootsQueryValidator()
        {
        }
    }
}