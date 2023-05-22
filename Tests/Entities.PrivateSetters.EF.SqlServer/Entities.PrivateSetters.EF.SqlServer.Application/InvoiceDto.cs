using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Application.Common.Mappings;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<LineDto> Lines { get; set; } = null!;
        public List<TagDto> Tags { get; set; } = null!;

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