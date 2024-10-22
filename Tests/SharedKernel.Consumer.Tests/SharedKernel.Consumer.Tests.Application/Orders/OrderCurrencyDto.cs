using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Application.Common.Mappings;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders
{
    public class OrderCurrencyDto : IMapFrom<Currency>
    {
        public OrderCurrencyDto()
        {
            Name = null!;
            Symbol = null!;
            Description = null!;
        }

        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }

        public static OrderCurrencyDto Create(Guid countryId, string name, string symbol, string description, Guid id)
        {
            return new OrderCurrencyDto
            {
                CountryId = countryId,
                Name = name,
                Symbol = symbol,
                Description = description,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Currency, OrderCurrencyDto>();
        }
    }
}