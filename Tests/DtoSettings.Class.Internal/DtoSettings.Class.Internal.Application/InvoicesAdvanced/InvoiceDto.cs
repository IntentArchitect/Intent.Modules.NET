using System;
using AutoMapper;
using DtoSettings.Class.Internal.Application.Common.Mappings;
using DtoSettings.Class.Internal.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.InvoicesAdvanced
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

        public Guid Id { get; internal set; }
        public string Number { get; internal set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}