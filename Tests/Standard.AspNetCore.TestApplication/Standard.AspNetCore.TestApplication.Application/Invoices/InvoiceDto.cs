using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Common.Mappings;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Invoices
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