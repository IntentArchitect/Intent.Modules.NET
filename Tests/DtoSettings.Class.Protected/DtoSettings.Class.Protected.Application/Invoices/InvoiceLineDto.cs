using System;
using AutoMapper;
using DtoSettings.Class.Protected.Application.Common.Mappings;
using DtoSettings.Class.Protected.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class InvoiceLineDto : IMapFrom<InvoiceLine>
    {
        public InvoiceLineDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; protected set; }
        public decimal Amount { get; protected set; }
        public string Currency { get; protected set; }
        public Guid InvoiceId { get; protected set; }
        public Guid Id { get; protected set; }

        public static InvoiceLineDto Create(string description, decimal amount, string currency, Guid invoiceId, Guid id)
        {
            return new InvoiceLineDto
            {
                Description = description,
                Amount = amount,
                Currency = currency,
                InvoiceId = invoiceId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceLineDto>();
        }
    }
}