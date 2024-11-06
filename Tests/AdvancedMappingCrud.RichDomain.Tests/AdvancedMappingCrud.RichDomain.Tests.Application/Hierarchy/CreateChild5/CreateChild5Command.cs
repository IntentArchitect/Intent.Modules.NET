using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild5
{
    public class CreateChild5Command : IRequest<Guid>, ICommand
    {
        public CreateChild5Command(Child5 dto)
        {
            Dto = dto;
        }

        public Child5 Dto { get; set; }
    }
}