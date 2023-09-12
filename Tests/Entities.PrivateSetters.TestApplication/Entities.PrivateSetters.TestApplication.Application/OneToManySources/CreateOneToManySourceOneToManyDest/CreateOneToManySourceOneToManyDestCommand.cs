using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.CreateOneToManySourceOneToManyDest
{
    public class CreateOneToManySourceOneToManyDestCommand : IRequest<Guid>, ICommand
    {
        public CreateOneToManySourceOneToManyDestCommand(string attribute, Guid ownerId)
        {
            Attribute = attribute;
            OwnerId = ownerId;
        }

        public string Attribute { get; set; }
        public Guid OwnerId { get; set; }
    }
}