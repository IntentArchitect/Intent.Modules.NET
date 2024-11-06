using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy.CreateChild4
{
    public class CreateChild4Command : IRequest<Guid>, ICommand
    {
        public CreateChild4Command(Child4 dto)
        {
            Dto = dto;
        }

        public Child4 Dto { get; set; }
    }
}