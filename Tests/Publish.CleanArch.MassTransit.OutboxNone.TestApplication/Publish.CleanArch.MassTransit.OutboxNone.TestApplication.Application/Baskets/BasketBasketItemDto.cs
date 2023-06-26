using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Mappings;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets
{
    public class BasketBasketItemDto : IMapFrom<BasketItem>
    {
        public BasketBasketItemDto()
        {
            Description = null!;
        }

        public Guid BasketId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public static BasketBasketItemDto Create(Guid basketId, Guid id, string description, decimal amount)
        {
            return new BasketBasketItemDto
            {
                BasketId = basketId,
                Id = id,
                Description = description,
                Amount = amount
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BasketItem, BasketBasketItemDto>();
        }
    }
}