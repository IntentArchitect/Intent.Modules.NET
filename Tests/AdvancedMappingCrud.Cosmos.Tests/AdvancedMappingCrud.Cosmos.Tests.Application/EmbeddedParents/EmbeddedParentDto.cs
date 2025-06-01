using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents
{
    public class EmbeddedParentDto : IMapFrom<EmbeddedParent>
    {
        public EmbeddedParentDto()
        {
            Id = null!;
            Name = null!;
            Children = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<EmbeddedEmbeddedParentEmbeddedChildDto> Children { get; set; }

        public static EmbeddedParentDto Create(string id, string name, List<EmbeddedEmbeddedParentEmbeddedChildDto> children)
        {
            return new EmbeddedParentDto
            {
                Id = id,
                Name = name,
                Children = children
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EmbeddedParent, EmbeddedParentDto>()
                .ForMember(d => d.Children, opt => opt.MapFrom(src => src.Children));

            profile.CreateMap<IEmbeddedParentDocument, EmbeddedParentDto>()
                .ForMember(d => d.Children, opt => opt.MapFrom(src => src.Children));
        }
    }
}