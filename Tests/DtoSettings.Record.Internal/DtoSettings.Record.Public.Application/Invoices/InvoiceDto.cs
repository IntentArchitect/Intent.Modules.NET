using System;
using System.Collections.Generic;
using AutoMapper;
using DtoSettings.Record.Public.Application.Common.Mappings;
using DtoSettings.Record.Public.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto(Guid id, string number, List<InvoiceLineDto> invoiceLines)
        {
            Id = id;
            Number = number;
            InvoiceLines = invoiceLines;
        }

        protected InvoiceDto()
        {
            Number = null!;
            InvoiceLines = null!;
        }

        public Guid Id { get; internal set; }
        public string Number { get; internal set; }
        public List<InvoiceLineDto> InvoiceLines { get; internal set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.InvoiceLines, opt => opt.MapFrom(src => src.InvoiceLines));
        }
    }
}