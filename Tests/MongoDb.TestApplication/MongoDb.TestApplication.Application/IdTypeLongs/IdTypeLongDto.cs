using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeLongs
{
    public class IdTypeLongDto : IMapFrom<IdTypeLong>
    {
        public IdTypeLongDto()
        {
        }

        public long Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeLongDto Create(long id, string attribute)
        {
            return new IdTypeLongDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdTypeLong, IdTypeLongDto>();
        }
    }
}