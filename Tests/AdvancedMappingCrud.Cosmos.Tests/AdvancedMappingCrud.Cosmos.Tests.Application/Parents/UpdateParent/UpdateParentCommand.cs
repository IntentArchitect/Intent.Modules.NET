using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents.UpdateParent
{
    public class UpdateParentCommand : IRequest, ICommand
    {
        public UpdateParentCommand(string name,
            string id,
            List<UpdateParentCommandChildrenDto>? children,
            UpdateParentCommandParentDetailsDto? parentDetails)
        {
            Name = name;
            Id = id;
            Children = children;
            ParentDetails = parentDetails;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public List<UpdateParentCommandChildrenDto>? Children { get; set; }
        public UpdateParentCommandParentDetailsDto? ParentDetails { get; set; }
    }
}