using System;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootComposite.GetAggregateRootComposite
{
    public class GetAggregateRootComposite : IRequest<CompositeSingleADTO>, IQuery
    {
        public Guid AggregateRootId { get; set; }

    }
}