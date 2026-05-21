using System;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.GiftCards
{
    public class GiftCardDto : IMapFrom<GiftCard>
    {
        public GiftCardDto()
        {
            Id = null!;
        }

        public string Id { get; set; }
        public decimal Value { get; set; }
        public Guid? CustomerId { get; set; }

        public static GiftCardDto Create(string id, decimal value, Guid? customerId)
        {
            return new GiftCardDto
            {
                Id = id,
                Value = value,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GiftCard, GiftCardDto>();
        }
    }
}