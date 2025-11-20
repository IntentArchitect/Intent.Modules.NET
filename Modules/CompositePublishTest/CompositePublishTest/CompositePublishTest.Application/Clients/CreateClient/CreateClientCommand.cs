using System;
using CompositePublishTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CompositePublishTest.Application.Clients.CreateClient
{
    public class CreateClientCommand : IRequest<Guid>, ICommand
    {
        public CreateClientCommand(string name, string location, string description)
        {
            Name = name;
            Location = location;
            Description = description;
        }

        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}