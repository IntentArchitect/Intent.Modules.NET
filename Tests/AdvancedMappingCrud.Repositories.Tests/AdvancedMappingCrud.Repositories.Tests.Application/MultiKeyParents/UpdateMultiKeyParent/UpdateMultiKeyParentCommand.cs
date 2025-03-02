using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.UpdateMultiKeyParent
{
    public class UpdateMultiKeyParentCommand : IRequest, ICommand
    {
        public UpdateMultiKeyParentCommand(string name,
            Guid id,
            List<UpdateMultiKeyParentCommandMultiKeyChildrenDto> multiKeyChildren)
        {
            Name = name;
            Id = id;
            MultiKeyChildren = multiKeyChildren;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<UpdateMultiKeyParentCommandMultiKeyChildrenDto> MultiKeyChildren { get; set; }
    }
}