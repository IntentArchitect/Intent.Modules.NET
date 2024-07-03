using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.DeleteClientsServices.DeleteClient
{
    public class DeleteClientCommand : IRequest, ICommand
    {
        public DeleteClientCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}