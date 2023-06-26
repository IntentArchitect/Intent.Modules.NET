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
    public class BasketDto : IMapFrom<Basket>
    {
        public BasketDto()
        {
            Number = null!;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }

        public static BasketDto Create(Guid id, string number)
        {
            return new BasketDto
            {
                Id = id,
                Number = number
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Basket, BasketDto>();
        }
    }
}