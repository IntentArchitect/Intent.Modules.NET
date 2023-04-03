using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeInts
{
    public class IdTypeIntDto : IMapFrom<IdTypeInt>
    {
        public IdTypeIntDto()
        {
        }

        public int Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeIntDto Create(int id, string attribute)
        {
            return new IdTypeIntDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdTypeInt, IdTypeIntDto>();
        }
    }
}