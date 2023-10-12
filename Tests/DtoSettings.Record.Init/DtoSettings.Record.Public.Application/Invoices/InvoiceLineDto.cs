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
        public InvoiceLineDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; }
        public Guid InvoiceId { get; init; }
        public Guid Id { get; init; }

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