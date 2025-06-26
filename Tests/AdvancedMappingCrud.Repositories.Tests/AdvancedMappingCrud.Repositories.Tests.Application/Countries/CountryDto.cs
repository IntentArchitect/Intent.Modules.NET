using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    public class CountryDto : IMapFrom<Country>
    {
        public CountryDto()
        {
            MaE = null!;
        }

        public Guid Id { get; set; }
        public string MaE { get; set; }

        public static CountryDto Create(Guid id, string maE)
        {
            return new CountryDto
            {
                Id = id,
                MaE = maE
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Country, CountryDto>();
        }
    }
}