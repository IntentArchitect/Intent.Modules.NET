using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild1
{
    public class CreateChild1Command : IRequest<Guid>, ICommand
    {
        public CreateChild1Command(Child1 dto)
        {
            Dto = dto;
        }

        public Child1 Dto { get; set; }
    }
}