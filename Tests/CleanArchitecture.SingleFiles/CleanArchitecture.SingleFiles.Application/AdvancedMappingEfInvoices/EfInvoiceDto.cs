using System;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingEfInvoices
{
    public class EfInvoiceDto : IMapFrom<EfInvoice>
    {
        public EfInvoiceDto()
        {
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }

        public static EfInvoiceDto Create(Guid id, string description)
        {
            return new EfInvoiceDto
            {
                Id = id,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EfInvoice, EfInvoiceDto>();
        }
    }
}