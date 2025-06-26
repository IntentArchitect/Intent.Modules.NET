using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
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