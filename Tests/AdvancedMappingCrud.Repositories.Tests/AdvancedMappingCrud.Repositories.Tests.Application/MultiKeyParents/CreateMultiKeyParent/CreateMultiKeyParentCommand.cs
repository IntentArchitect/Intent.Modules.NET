using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.CreateMultiKeyParent
{
    public class CreateMultiKeyParentCommand : IRequest<Guid>, ICommand
    {
        public CreateMultiKeyParentCommand(string name,
            List<CreateMultiKeyParentCommandMultiKeyChildrenDto> multiKeyChildren)
        {
            Name = name;
            MultiKeyChildren = multiKeyChildren;
        }

        public string Name { get; set; }
        public List<CreateMultiKeyParentCommandMultiKeyChildrenDto> MultiKeyChildren { get; set; }
    }
}