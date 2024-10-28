using System;
using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.DeleteAggregateRootCompositeManyB
{
    public class DeleteAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public DeleteAggregateRootCompositeManyBCommand(Guid aggregateRootId, Guid id)
        {
            AggregateRootId = aggregateRootId;
            Id = id;
        }

        [FromQueryParams]
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
    }
}