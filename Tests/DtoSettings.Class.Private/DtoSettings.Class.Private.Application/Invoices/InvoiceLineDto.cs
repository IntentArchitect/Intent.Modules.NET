using System;
using AutoMapper;
using DtoSettings.Class.Private.Application.Common.Mappings;
using DtoSettings.Class.Private.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Invoices
{
    public class InvoiceLineDto : IMapFrom<InvoiceLine>
    {
        public InvoiceLineDto(string description, decimal amount, string currency, Guid invoiceId, Guid id)
        {
            Description = description;
            Amount = amount;
            Currency = currency;
            InvoiceId = invoiceId;
            Id = id;
        }

        protected InvoiceLineDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public Guid InvoiceId { get; private set; }
        public Guid Id { get; private set; }

        public static InvoiceLineDto Create(string description, decimal amount, string currency, Guid invoiceId, Guid id)
        {
            return new InvoiceLineDto(description, amount, currency, invoiceId, id);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceLineDto>();
        }
    }
}