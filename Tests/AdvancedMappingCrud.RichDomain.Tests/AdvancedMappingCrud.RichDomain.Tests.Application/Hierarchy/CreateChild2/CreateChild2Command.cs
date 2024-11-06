using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild2
{
    public class CreateChild2Command : IRequest<Guid>, ICommand
    {
        public CreateChild2Command(Child2 dto)
        {
            Dto = dto;
        }

        public Child2 Dto { get; set; }
    }
}