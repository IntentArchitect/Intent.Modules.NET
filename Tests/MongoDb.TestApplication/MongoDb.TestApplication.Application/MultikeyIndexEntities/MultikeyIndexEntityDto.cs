using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    public class MultikeyIndexEntityDto : IMapFrom<MultikeyIndexEntity>
    {
        public MultikeyIndexEntityDto()
        {
            Id = null!;
            MultiKey = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public List<string> MultiKey { get; set; }
        public string SomeField { get; set; }

        public static MultikeyIndexEntityDto Create(string id, List<string> multiKey, string someField)
        {
            return new MultikeyIndexEntityDto
            {
                Id = id,
                MultiKey = multiKey,
                SomeField = someField
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MultikeyIndexEntity, MultikeyIndexEntityDto>();
        }
    }
}