using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild3
{
    public class CreateChild3Command : IRequest<Guid>, ICommand
    {
        public CreateChild3Command(Child3 dto)
        {
            Dto = dto;
        }

        public Child3 Dto { get; set; }
    }
}