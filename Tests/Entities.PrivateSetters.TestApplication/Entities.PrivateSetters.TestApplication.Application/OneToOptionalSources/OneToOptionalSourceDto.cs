using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToOptionalSources
{
    public class OneToOptionalSourceDto : IMapFrom<OneToOptionalSource>
    {
        public OneToOptionalSourceDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static OneToOptionalSourceDto Create(Guid id, string attribute)
        {
            return new OneToOptionalSourceDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OneToOptionalSource, OneToOptionalSourceDto>();
        }
    }
}