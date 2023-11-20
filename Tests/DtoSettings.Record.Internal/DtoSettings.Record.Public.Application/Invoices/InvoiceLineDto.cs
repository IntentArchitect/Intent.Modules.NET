using System;
using AutoMapper;
using DtoSettings.Record.Public.Application.Common.Mappings;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceLineDto : IMapFrom<InvoiceLine>
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

        public string Description { get; internal set; }
        public decimal Amount { get; internal set; }
        public string Currency { get; internal set; }
        public Guid InvoiceId { get; internal set; }
        public Guid Id { get; internal set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceLineDto>();
        }
    }
}