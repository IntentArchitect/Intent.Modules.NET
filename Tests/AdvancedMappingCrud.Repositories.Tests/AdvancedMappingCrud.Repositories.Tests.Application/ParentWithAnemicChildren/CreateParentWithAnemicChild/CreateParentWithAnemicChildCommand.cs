using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.CreateParentWithAnemicChild
{
    public class CreateParentWithAnemicChildCommand : IRequest<Guid>, ICommand
    {
        public CreateParentWithAnemicChildCommand(string name,
            string surname,
            List<CreateParentWithAnemicChildAnemicChildDto> anemicChildren)
        {
            Name = name;
            Surname = surname;
            AnemicChildren = anemicChildren;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public List<CreateParentWithAnemicChildAnemicChildDto> AnemicChildren { get; set; }
    }
}