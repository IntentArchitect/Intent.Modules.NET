using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    public class IdTypeGuidDto : IMapFrom<IdTypeGuid>
    {
        public IdTypeGuidDto()
        {
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeGuidDto Create(Guid id, string attribute)
        {
            return new IdTypeGuidDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdTypeGuid, IdTypeGuidDto>();
        }
    }
}