using System;
using AutoMapper;
using DtoSettings.Class.Private.Application.Common.Mappings;
using DtoSettings.Class.Private.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.InvoicesAdvanced
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto(Guid id, string number)
        {
            Id = id;
            Number = number;
        }

        protected InvoiceDto()
        {
            Number = null!;
        }

        public Guid Id { get; private set; }
        public string Number { get; private set; }

        public static InvoiceDto Create(Guid id, string number)
        {
            return new InvoiceDto(id, number);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}