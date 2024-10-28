using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.UpdateAggregateRootCompositeManyB
{
    public class UpdateAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public UpdateAggregateRootCompositeManyBCommand(Guid aggregateRootId,
            string compositeAttr,
            DateTime? someDate,
            Guid id)
        {
            AggregateRootId = aggregateRootId;
            CompositeAttr = compositeAttr;
            SomeDate = someDate;
            Id = id;
        }

        public Guid AggregateRootId { get; set; }
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public Guid Id { get; set; }
    }
}