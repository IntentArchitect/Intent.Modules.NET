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
        }

        public string Id { get; set; } = null!;
        public DateTime Date { get; set; }
        public IEnumerable<string> TagsIds { get; set; } = null!;
        public List<LineDto> Lines { get; set; } = null!;
        public List<TagDto> Tags { get; set; } = null!;

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
                .AfterMap<TagsMappingAction>();
        }

        internal class TagsMappingAction : IMappingAction<Invoice, InvoiceDto>
        {
            private readonly ITagRepository _repository;
            private readonly IMapper _mapper;

            public TagsMappingAction(ITagRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public void Process(Invoice source, InvoiceDto destination, ResolutionContext context)
            {
                var entity = _repository.FindByIdsAsync(source.TagsIds.ToArray()).Result;
                destination.Tags = entity.MapToTagDtoList(_mapper);
            }
        }
    }
}