using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.GetAggregateTestNoIdReturnById
{
    public class GetAggregateTestNoIdReturnByIdQuery : IRequest<AggregateTestNoIdReturnDto>, IQuery
    {
        public GetAggregateTestNoIdReturnByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}