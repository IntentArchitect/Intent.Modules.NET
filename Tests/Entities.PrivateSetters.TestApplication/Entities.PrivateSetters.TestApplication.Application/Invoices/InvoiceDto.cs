using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public static InvoiceDto Create(Guid id, DateTime date)
        {
            return new InvoiceDto
            {
                Id = id,
                Date = date
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}