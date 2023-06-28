using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Mappings;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public class PurchaseMoneyDto : IMapFrom<Money>
    {
        public PurchaseMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static PurchaseMoneyDto Create(decimal amount, string currency)
        {
            return new PurchaseMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Money, PurchaseMoneyDto>();
        }
    }
}