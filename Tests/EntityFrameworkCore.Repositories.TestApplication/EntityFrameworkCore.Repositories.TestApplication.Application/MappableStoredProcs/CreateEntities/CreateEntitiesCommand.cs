using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.CreateEntities
{
    public class CreateEntitiesCommand : IRequest, ICommand
    {
        public CreateEntitiesCommand(List<CreateEntitiesCommandentitiesDto> entities)
        {
            Entities = entities;
        }

        public List<CreateEntitiesCommandentitiesDto> Entities { get; set; }
    }
}