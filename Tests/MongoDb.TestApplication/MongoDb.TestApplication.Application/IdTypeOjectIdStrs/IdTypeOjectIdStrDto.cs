using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeOjectIdStrs
{
    public class IdTypeOjectIdStrDto : IMapFrom<IdTypeOjectIdStr>
    {
        public IdTypeOjectIdStrDto()
        {
        }

        public string Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeOjectIdStrDto Create(string id, string attribute)
        {
            return new IdTypeOjectIdStrDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdTypeOjectIdStr, IdTypeOjectIdStrDto>();
        }
    }
}