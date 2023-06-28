using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public class PurchaseDto : IMapFrom<Purchase>
    {
        public PurchaseDto()
        {
            Cost = null!;
        }

        public Guid Id { get; set; }
        public PurchaseMoneyDto Cost { get; set; }

        public static PurchaseDto Create(Guid id, PurchaseMoneyDto cost)
        {
            return new PurchaseDto
            {
                Id = id,
                Cost = cost
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Purchase, PurchaseDto>();
        }
    }
}