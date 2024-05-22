using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents.CreateNestingParent
{
    public class CreateNestingParentCommand : IRequest<Guid>, ICommand
    {
        public CreateNestingParentCommand(string name, List<CreateNestingParentCommandNestingChildrenDto> nestingChildren)
        {
            Name = name;
            NestingChildren = nestingChildren;
        }

        public string Name { get; set; }
        public List<CreateNestingParentCommandNestingChildrenDto> NestingChildren { get; set; }
    }
}