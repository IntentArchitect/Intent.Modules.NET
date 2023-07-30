using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneSources
{
    public class OptionalToOneSourceDto : IMapFrom<OptionalToOneSource>
    {
        public OptionalToOneSourceDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static OptionalToOneSourceDto Create(Guid id, string attribute)
        {
            return new OptionalToOneSourceDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OptionalToOneSource, OptionalToOneSourceDto>();
        }
    }
}