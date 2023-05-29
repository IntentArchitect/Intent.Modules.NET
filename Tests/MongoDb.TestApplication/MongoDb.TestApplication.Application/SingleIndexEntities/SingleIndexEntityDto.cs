using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    public class SingleIndexEntityDto : IMapFrom<SingleIndexEntity>
    {
        public SingleIndexEntityDto()
        {
            Id = null!;
            SomeField = null!;
            SingleIndex = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }
        public string SingleIndex { get; set; }

        public static SingleIndexEntityDto Create(string id, string someField, string singleIndex)
        {
            return new SingleIndexEntityDto
            {
                Id = id,
                SomeField = someField,
                SingleIndex = singleIndex
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SingleIndexEntity, SingleIndexEntityDto>();
        }
    }
}