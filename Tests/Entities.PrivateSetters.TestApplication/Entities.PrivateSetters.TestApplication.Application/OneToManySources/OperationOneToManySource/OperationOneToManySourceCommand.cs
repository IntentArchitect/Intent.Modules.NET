using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManySource
{
    public class OperationOneToManySourceCommand : IRequest, ICommand
    {
        public OperationOneToManySourceCommand(string attribute, Guid ownerId, Guid id)
        {
            Attribute = attribute;
            OwnerId = ownerId;
            Id = id;
        }

        public string Attribute { get; set; }
        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
    }
}