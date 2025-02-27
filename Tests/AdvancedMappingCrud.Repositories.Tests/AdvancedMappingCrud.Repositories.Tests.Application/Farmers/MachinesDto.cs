using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers
{
    public class MachinesDto : IMapFrom<Machines>
    {
        public MachinesDto()
        {
            Name = null!;
        }

        public Guid FarmerId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static MachinesDto Create(Guid farmerId, Guid id, string name)
        {
            return new MachinesDto
            {
                FarmerId = farmerId,
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Machines, MachinesDto>();
        }
    }
}