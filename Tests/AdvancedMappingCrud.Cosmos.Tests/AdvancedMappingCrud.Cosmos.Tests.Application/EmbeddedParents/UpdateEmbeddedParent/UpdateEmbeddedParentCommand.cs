using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.UpdateEmbeddedParent
{
    public class UpdateEmbeddedParentCommand : IRequest, ICommand
    {
        public UpdateEmbeddedParentCommand(string name, List<UpdateEmbeddedParentEmbeddedChildDto> children, string id)
        {
            Name = name;
            Children = children;
            Id = id;
        }

        public string Name { get; set; }
        public List<UpdateEmbeddedParentEmbeddedChildDto> Children { get; set; }
        public string Id { get; set; }
    }
}