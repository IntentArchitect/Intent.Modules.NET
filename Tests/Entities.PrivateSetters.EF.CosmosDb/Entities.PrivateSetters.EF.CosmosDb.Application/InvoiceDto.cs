using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.CosmosDb.Application.Common.Mappings;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Lines = null!;
            Tags = null!;
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<LineDto> Lines { get; set; }
        public List<TagDto> Tags { get; set; }

        public static InvoiceDto Create(Guid id, DateTime date, List<LineDto> lines, List<TagDto> tags)
        {
            return new InvoiceDto
            {
                Id = id,
                Date = date,
                Lines = lines,
                Tags = tags
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.Lines, opt => opt.MapFrom(src => src.Lines))
                .ForMember(d => d.Tags, opt => opt.MapFrom(src => src.Tags));
        }
    }
}