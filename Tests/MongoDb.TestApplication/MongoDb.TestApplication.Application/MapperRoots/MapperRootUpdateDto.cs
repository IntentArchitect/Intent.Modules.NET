using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapperRootUpdateDto
    {
        public MapperRootUpdateDto()
        {
            Id = null!;
            No = null!;
            MapAggChildrenIds = null!;
            MapAggPeerId = null!;
        }

        public string Id { get; set; }
        public string No { get; set; }
        public List<string> MapAggChildrenIds { get; set; }
        public string MapAggPeerId { get; set; }

        public static MapperRootUpdateDto Create(string id, string no, List<string> mapAggChildrenIds, string mapAggPeerId)
        {
            return new MapperRootUpdateDto
            {
                Id = id,
                No = no,
                MapAggChildrenIds = mapAggChildrenIds,
                MapAggPeerId = mapAggPeerId
            };
        }
    }
}