using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.CreateEmbeddedParent
{
    public class CreateEmbeddedParentCommand : IRequest<string>, ICommand
    {
        public CreateEmbeddedParentCommand(string name, List<CreateEmbeddedParentEmbeddedChildDto> children)
        {
            Name = name;
            Children = children;
        }

        public string Name { get; set; }
        public List<CreateEmbeddedParentEmbeddedChildDto> Children { get; set; }
    }
}