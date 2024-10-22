using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Application.Common.Mappings;
using SharedKernel.Consumer.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
            RefNo = null!;
            Country = null!;
            Currency = null!;
        }

        public Guid Id { get; set; }
        public string RefNo { get; set; }
        public Guid CountryId { get; set; }
        public OrderCountryDto Country { get; set; }
        public OrderCurrencyDto Currency { get; set; }

        public static OrderDto Create(Guid id, string refNo, Guid countryId, OrderCountryDto country, OrderCurrencyDto currency)
        {
            return new OrderDto
            {
                Id = id,
                RefNo = refNo,
                CountryId = countryId,
                Country = country,
                Currency = currency
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}