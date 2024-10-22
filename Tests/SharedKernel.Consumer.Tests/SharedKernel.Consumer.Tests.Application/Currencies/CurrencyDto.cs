using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Application.Common.Mappings;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies
{
    public class CurrencyDto : IMapFrom<Currency>
    {
        public CurrencyDto()
        {
            Name = null!;
            Symbol = null!;
            Description = null!;
        }

        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }

        public static CurrencyDto Create(Guid id, Guid countryId, string name, string symbol, string description)
        {
            return new CurrencyDto
            {
                Id = id,
                CountryId = countryId,
                Name = name,
                Symbol = symbol,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Currency, CurrencyDto>();
        }
    }
}