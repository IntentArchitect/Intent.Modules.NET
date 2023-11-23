using System;
using System.Collections.Generic;
using AutoMapper;
using DtoSettings.Class.Protected.Application.Common.Mappings;
using DtoSettings.Class.Protected.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
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

        public Guid Id { get; protected set; }
        public string Number { get; protected set; }
        public List<InvoiceLineDto> InvoiceLines { get; protected set; }

        public static InvoiceDto Create(Guid id, string number, List<InvoiceLineDto> invoiceLines)
        {
            return new InvoiceDto(id, number, invoiceLines);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.InvoiceLines, opt => opt.MapFrom(src => src.InvoiceLines));
        }
    }
}