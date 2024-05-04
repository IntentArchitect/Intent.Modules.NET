using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices
{
    public class InvoiceInvoiceLineDto : IMapFrom<InvoiceLine>
    {
        public InvoiceInvoiceLineDto()
        {
            InvoiceId = null!;
            Id = null!;
            Description = null!;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public static InvoiceInvoiceLineDto Create(string invoiceId, string id, string description, int quantity)
        {
            return new InvoiceInvoiceLineDto
            {
                InvoiceId = invoiceId,
                Id = id,
                Description = description,
                Quantity = quantity
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceInvoiceLineDto>();
        }
    }
}