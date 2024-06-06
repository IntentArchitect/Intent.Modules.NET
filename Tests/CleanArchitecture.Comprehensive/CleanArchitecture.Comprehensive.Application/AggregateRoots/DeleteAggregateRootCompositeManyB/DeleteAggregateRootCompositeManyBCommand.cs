using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRootCompositeManyB
{
    public class DeleteAggregateRootCompositeManyBCommand : IRequest, ICommand
    {
        public DeleteAggregateRootCompositeManyBCommand(Guid aggregateRootId, Guid id)
        {
            AggregateRootId = aggregateRootId;
            Id = id;
        }

        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
    }
}