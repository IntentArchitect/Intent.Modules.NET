using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManyDests
{
    public class ManyToManyDestDto : IMapFrom<ManyToManyDest>
    {
        public ManyToManyDestDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static ManyToManyDestDto Create(Guid id, string attribute)
        {
            return new ManyToManyDestDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ManyToManyDest, ManyToManyDestDto>();
        }
    }
}