using System;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany.GetAggregateRootCompositeManyById
{
    public class GetAggregateRootCompositeManyByIdQuery : IRequest<CompositeManyBDTO>, IQuery
    {
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }

    }
}