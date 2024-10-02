using System;
using System.Collections.Generic;
using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Invoices
{
    public class InvoiceLineItemDto : IMapFrom<LineItem>
    {
        public InvoiceLineItemDto()
        {
            Id = null!;
            Description = null!;
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public static InvoiceLineItemDto Create(string id, string description, int quantity)
        {
            return new InvoiceLineItemDto
            {
                Id = id,
                Description = description,
                Quantity = quantity
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LineItem, InvoiceLineItemDto>();
        }
    }
}