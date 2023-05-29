using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    public static class SingleIndexEntitySingleParentDtoMappingExtensions
    {
        public static SingleIndexEntitySingleParentDto MapToSingleIndexEntitySingleParentDto(this SingleIndexEntitySingleParent projectFrom, IMapper mapper)
            => mapper.Map<SingleIndexEntitySingleParentDto>(projectFrom);

        public static List<SingleIndexEntitySingleParentDto> MapToSingleIndexEntitySingleParentDtoList(this IEnumerable<SingleIndexEntitySingleParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSingleIndexEntitySingleParentDto(mapper)).ToList();
    }
}