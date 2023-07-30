using System;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Application.Common.Mappings;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToManyDests
{
    public class OptionalToManyDestDto : IMapFrom<OptionalToManyDest>
    {
        public OptionalToManyDestDto()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }
        public Guid? OptionalOneToManySourceId { get; set; }
        public string Attribute { get; set; }

        public static OptionalToManyDestDto Create(Guid id, Guid? optionalOneToManySourceId, string attribute)
        {
            return new OptionalToManyDestDto
            {
                Id = id,
                OptionalOneToManySourceId = optionalOneToManySourceId,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OptionalToManyDest, OptionalToManyDestDto>();
        }
    }
}