using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Mappings;
using Kafka.Consumer.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Kafka.Consumer.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Note = null!;
        }

        public Guid Id { get; set; }
        public string Note { get; set; }

        public static InvoiceDto Create(Guid id, string note)
        {
            return new InvoiceDto
            {
                Id = id,
                Note = note
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}