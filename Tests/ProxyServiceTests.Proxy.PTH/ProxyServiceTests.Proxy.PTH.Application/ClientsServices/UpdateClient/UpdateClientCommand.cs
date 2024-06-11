using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.PTH.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.ClientsServices.UpdateClient
{
    public class UpdateClientCommand : IRequest, ICommand
    {
        public UpdateClientCommand(Guid id, Guid clientUpdateDtoId, string name)
        {
            Id = id;
            ClientUpdateDtoId = clientUpdateDtoId;
            Name = name;
        }

        public Guid Id { get; set; }
        public Guid ClientUpdateDtoId { get; set; }
        public string Name { get; set; }
    }
}