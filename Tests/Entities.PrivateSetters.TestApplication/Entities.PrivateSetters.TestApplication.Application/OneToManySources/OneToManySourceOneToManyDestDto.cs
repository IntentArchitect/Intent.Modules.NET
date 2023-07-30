using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources
{
    public class OneToManySourceOneToManyDestDto : IMapFrom<OneToManyDest>
    {
        public OneToManySourceOneToManyDestDto()
        {
            Attribute = null!;
        }

        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static OneToManySourceOneToManyDestDto Create(Guid ownerId, Guid id, string attribute)
        {
            return new OneToManySourceOneToManyDestDto
            {
                OwnerId = ownerId,
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OneToManyDest, OneToManySourceOneToManyDestDto>();
        }
    }
}