using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS.GetAggregateRootCompositeManyBS
{
    public class GetAggregateRootCompositeManyBSQuery : IRequest<List<AggregateRootCompositeManyBDTO>>, IQuery
    {
        public Guid AggregateRootId { get; set; }

    }
}