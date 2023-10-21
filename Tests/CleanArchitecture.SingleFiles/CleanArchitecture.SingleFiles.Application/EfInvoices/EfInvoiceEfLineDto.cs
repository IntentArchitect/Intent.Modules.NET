using System;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class EfInvoiceEfLineDto : IMapFrom<EfLine>
    {
        public EfInvoiceEfLineDto()
        {
            Name = null!;
        }

        public Guid EfInvoicesId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static EfInvoiceEfLineDto Create(Guid efInvoicesId, Guid id, string name)
        {
            return new EfInvoiceEfLineDto
            {
                EfInvoicesId = efInvoicesId,
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EfLine, EfInvoiceEfLineDto>();
        }
    }
}