using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources
{
    public class ManyToManySourceDto : IMapFrom<ManyToManySource>
    {
        public ManyToManySourceDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static ManyToManySourceDto Create(Guid id, string attribute)
        {
            return new ManyToManySourceDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ManyToManySource, ManyToManySourceDto>();
        }
    }
}