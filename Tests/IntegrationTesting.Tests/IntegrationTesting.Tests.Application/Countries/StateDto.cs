using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public class StateDto : IMapFrom<State>
    {
        public StateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }

        public static StateDto Create(Guid id, string name, Guid countryId)
        {
            return new StateDto
            {
                Id = id,
                Name = name,
                CountryId = countryId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<State, StateDto>();
        }
    }
}