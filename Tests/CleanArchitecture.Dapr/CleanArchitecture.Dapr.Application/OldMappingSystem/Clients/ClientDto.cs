using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Application.Common.Mappings;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients
{
    public class ClientDto : IMapFrom<Client>
    {
        public ClientDto()
        {
            Id = null!;
            Name = null!;
            Tags = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<ClientTagDto> Tags { get; set; }

        public static ClientDto Create(string id, string name, List<ClientTagDto> tags)
        {
            return new ClientDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Client, ClientDto>()
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<Client, ClientDto>
        {
            private readonly ITagRepository _tagRepository;
            private readonly IMapper _mapper;

            public MappingAction(ITagRepository tagRepository, IMapper mapper)
            {
                _tagRepository = tagRepository;
                _mapper = mapper;
            }

            public void Process(Client source, ClientDto destination, ResolutionContext context)
            {
                var tags = _tagRepository.FindByIdsAsync(source.TagsIds.ToArray()).Result;
                destination.Tags = tags.MapToClientTagDtoList(_mapper);
            }
        }
    }
}