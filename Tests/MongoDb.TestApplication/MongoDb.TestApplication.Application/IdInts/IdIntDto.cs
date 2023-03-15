using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdInts
{

    public class IdIntDto : IMapFrom<IdInt>
    {
        public IdIntDto()
        {
        }

        public static IdIntDto Create(
            int id,
            string attribute)
        {
            return new IdIntDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public int Id { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IdInt, IdIntDto>();
        }
    }
}