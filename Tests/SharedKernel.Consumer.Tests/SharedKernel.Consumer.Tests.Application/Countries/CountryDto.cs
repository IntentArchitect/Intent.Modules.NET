using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Application.Common.Mappings;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries
{
    public class CountryDto : IMapFrom<Country>
    {
        public CountryDto()
        {
            Name = null!;
            Code = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public static CountryDto Create(Guid id, string name, string code)
        {
            return new CountryDto
            {
                Id = id,
                Name = name,
                Code = code
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Country, CountryDto>();
        }
    }
}