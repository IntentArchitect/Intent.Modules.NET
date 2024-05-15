using System.Collections.Generic;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients.CreateClient
{
    public class CreateClientCommand : IRequest<string>, ICommand
    {
        public CreateClientCommand(string name, List<string> tagsIds)
        {
            Name = name;
            TagsIds = tagsIds;
        }

        public string Name { get; set; }
        public List<string> TagsIds { get; set; }
    }
}