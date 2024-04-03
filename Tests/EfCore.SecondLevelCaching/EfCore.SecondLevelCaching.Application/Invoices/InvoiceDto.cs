using System;
using AutoMapper;
using EfCore.SecondLevelCaching.Application.Common.Mappings;
using EfCore.SecondLevelCaching.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Notes = null!;
        }

        public Guid Id { get; set; }
        public string Notes { get; set; }

        public static InvoiceDto Create(Guid id, string notes)
        {
            return new InvoiceDto
            {
                Id = id,
                Notes = notes
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}