using System;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.DeleteAggregateWithUniqueConstraintIndexStereotype
{
    public class DeleteAggregateWithUniqueConstraintIndexStereotypeCommand : IRequest, ICommand
    {
        public DeleteAggregateWithUniqueConstraintIndexStereotypeCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}