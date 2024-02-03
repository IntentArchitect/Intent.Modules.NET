using System;
using AutoMapper;
using EntityFrameworkCore.Oracle.TestApplication.Application.Common.Mappings;
using EntityFrameworkCore.Oracle.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Number = null!;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }

        public static InvoiceDto Create(Guid id, string number)
        {
            return new InvoiceDto
            {
                Id = id,
                Number = number
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}