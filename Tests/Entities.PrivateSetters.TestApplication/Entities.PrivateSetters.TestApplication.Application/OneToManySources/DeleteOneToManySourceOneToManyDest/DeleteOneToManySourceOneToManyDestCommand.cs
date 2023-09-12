using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.DeleteOneToManySourceOneToManyDest
{
    public class DeleteOneToManySourceOneToManyDestCommand : IRequest, ICommand
    {
        public DeleteOneToManySourceOneToManyDestCommand(Guid ownerId, Guid id)
        {
            OwnerId = ownerId;
            Id = id;
        }

        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
    }
}