using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
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