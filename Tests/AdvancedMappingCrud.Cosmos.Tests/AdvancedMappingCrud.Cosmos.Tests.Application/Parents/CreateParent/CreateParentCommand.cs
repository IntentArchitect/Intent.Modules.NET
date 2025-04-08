using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.CreateParent
{
    public class CreateParentCommand : IRequest<string>, ICommand
    {
        public CreateParentCommand(string name,
            List<CreateParentCommandChildrenDto>? children,
            CreateParentCommandParentDetailsDto? parentDetails)
        {
            Name = name;
            Children = children;
            ParentDetails = parentDetails;
        }

        public string Name { get; set; }
        public List<CreateParentCommandChildrenDto>? Children { get; set; }
        public CreateParentCommandParentDetailsDto? ParentDetails { get; set; }
    }
}