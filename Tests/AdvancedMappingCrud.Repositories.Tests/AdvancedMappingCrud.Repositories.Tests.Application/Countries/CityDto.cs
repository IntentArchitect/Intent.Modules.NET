using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    public class CityDto : IMapFrom<City>
    {
        public CityDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid StateId { get; set; }

        public static CityDto Create(Guid id, string name, Guid stateId)
        {
            return new CityDto
            {
                Id = id,
                Name = name,
                StateId = stateId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<City, CityDto>();
        }
    }
}