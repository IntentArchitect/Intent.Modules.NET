using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources
{
    public class ManyToOneSourceDto : IMapFrom<ManyToOneSource>
    {
        public ManyToOneSourceDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public Guid ManyToOneDestId { get; set; }
        public string Attribute { get; set; }

        public static ManyToOneSourceDto Create(Guid id, Guid manyToOneDestId, string attribute)
        {
            return new ManyToOneSourceDto
            {
                Id = id,
                ManyToOneDestId = manyToOneDestId,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ManyToOneSource, ManyToOneSourceDto>();
        }
    }
}