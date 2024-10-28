using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.DeleteAggregateWithUniqueConstraintIndexElement
{
    public class DeleteAggregateWithUniqueConstraintIndexElementCommand : IRequest, ICommand
    {
        public DeleteAggregateWithUniqueConstraintIndexElementCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}