using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapAggChildDto : IMapFrom<MapAggChild>
    {
        public MapAggChildDto()
        {
            ChildName = null!;
            Id = null!;
        }

        public string ChildName { get; set; }
        public string Id { get; set; }

        public static MapAggChildDto Create(string childName, string id)
        {
            return new MapAggChildDto
            {
                ChildName = childName,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MapAggChild, MapAggChildDto>();
        }
    }
}