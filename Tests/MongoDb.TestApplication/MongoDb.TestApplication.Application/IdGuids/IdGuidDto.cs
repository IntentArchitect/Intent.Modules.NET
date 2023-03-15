using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdGuids
{

    public class IdGuidDto : IMapFrom<IdGuid>
    {
        public IdGuidDto()
        {
        }

        public static IdGuidDto Create(
            Guid id,
            string attribute)
        {
            return new IdGuidDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdGuid, IdGuidDto>();
        }
    }
}