using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application.Common.Mappings;
using Solace.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Solace.Tests.Application.Purchases
{
    public class PurchaseDto : IMapFrom<Purchase>
    {
        public PurchaseDto()
        {
        }

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public static PurchaseDto Create(Guid id, Guid accountId, decimal amount)
        {
            return new PurchaseDto
            {
                Id = id,
                AccountId = accountId,
                Amount = amount
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Purchase, PurchaseDto>();
        }
    }
}