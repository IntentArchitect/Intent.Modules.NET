using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Common.Mappings;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            Id = null!;
            TagsIds = null!;
            Lines = null!;
            Tags = null!;
        }

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<string> TagsIds { get; set; }
        public List<LineDto> Lines { get; set; }
        public List<TagDto> Tags { get; set; }

        public static InvoiceDto Create(
            string id,
            DateTime date,
            IEnumerable<string> tagsIds,
            List<LineDto> lines,
            List<TagDto> tags)
        {
            return new InvoiceDto
            {
                Id = id,
                Date = date,
                TagsIds = tagsIds,
                Lines = lines,
                Tags = tags
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.Lines, opt => opt.MapFrom(src => src.Lines))
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<Invoice, InvoiceDto>
        {
            private readonly ITagRepository _tagRepository;
            private readonly IMapper _mapper;

            public MappingAction(ITagRepository tagRepository, IMapper mapper)
            {
                _tagRepository = tagRepository;
                _mapper = mapper;
            }

            public void Process(Invoice source, InvoiceDto destination, ResolutionContext context)
            {
                var tags = _tagRepository.FindByIdsAsync(source.TagsIds.ToArray()).Result;
                destination.Tags = tags.MapToTagDtoList(_mapper);
            }
        }
    }
}