using System;
using AutoMapper;
using DtoSettings.Class.Init.Application.Common.Mappings;
using DtoSettings.Class.Init.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.InvoicesAdvanced
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Number = null!;
        }

        public Guid Id { get; init; }
        public string Number { get; init; }

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