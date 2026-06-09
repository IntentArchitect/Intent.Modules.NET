using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.GiftCards
{
    public class GiftCardDto : IMapFrom<GiftCard>
    {
        public GiftCardDto()
        {
            CardCode = null!;
        }

        public string CardCode { get; set; }
        public decimal Value { get; set; }
        public Guid? UserId { get; set; }

        public static GiftCardDto Create(string cardCode, decimal value, Guid? userId)
        {
            return new GiftCardDto
            {
                CardCode = cardCode,
                Value = value,
                UserId = userId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GiftCard, GiftCardDto>();
        }
    }
}