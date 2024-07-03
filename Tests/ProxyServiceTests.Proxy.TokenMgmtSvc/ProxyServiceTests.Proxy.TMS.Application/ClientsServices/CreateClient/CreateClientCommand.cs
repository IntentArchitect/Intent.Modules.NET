using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.TMS.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.CreateClient
{
    public class CreateClientCommand : IRequest<Guid>, ICommand
    {
        public CreateClientCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}