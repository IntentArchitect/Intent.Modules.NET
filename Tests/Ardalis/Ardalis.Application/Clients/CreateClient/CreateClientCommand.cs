using System;
using Ardalis.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Ardalis.Application.Clients.CreateClient
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