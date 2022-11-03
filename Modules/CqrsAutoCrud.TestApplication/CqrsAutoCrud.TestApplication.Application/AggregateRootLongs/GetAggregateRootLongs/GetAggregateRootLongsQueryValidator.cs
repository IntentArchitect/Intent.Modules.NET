using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs.GetAggregateRootLongs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongsQueryValidator : AbstractValidator<GetAggregateRootLongsQuery>
    {
        [IntentManaged(Mode.Fully)]
        public GetAggregateRootLongsQueryValidator()
        {
        }
    }
}