using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapperRootCreateDto
    {
        public MapperRootCreateDto()
        {
            No = null!;
            MapAggChildrenIds = null!;
            MapAggPeerId = null!;
        }

        public string No { get; set; }
        public IEnumerable<string> MapAggChildrenIds { get; set; }
        public string MapAggPeerId { get; set; }

        public static MapperRootCreateDto Create(string no, IEnumerable<string> mapAggChildrenIds, string mapAggPeerId)
        {
            return new MapperRootCreateDto
            {
                No = no,
                MapAggChildrenIds = mapAggChildrenIds,
                MapAggPeerId = mapAggPeerId
            };
        }
    }
}