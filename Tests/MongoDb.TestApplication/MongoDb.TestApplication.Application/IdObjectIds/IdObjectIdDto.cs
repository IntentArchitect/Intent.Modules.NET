using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdObjectIds
{

    public class IdObjectIdDto : IMapFrom<IdObjectId>
    {
        public IdObjectIdDto()
        {
        }

        public static IdObjectIdDto Create(
            string id,
            string attribute)
        {
            return new IdObjectIdDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdObjectId, IdObjectIdDto>();
        }
    }
}