using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapMapMeDto : IMapFrom<MapMapMe>
    {
        public MapMapMeDto()
        {
            Name = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public static MapMapMeDto Create(string name, string id)
        {
            return new MapMapMeDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MapMapMe, MapMapMeDto>();
        }
    }
}