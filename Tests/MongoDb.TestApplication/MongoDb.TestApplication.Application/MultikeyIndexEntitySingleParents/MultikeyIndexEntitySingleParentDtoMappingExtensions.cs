using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    public static class MultikeyIndexEntitySingleParentDtoMappingExtensions
    {
        public static MultikeyIndexEntitySingleParentDto MapToMultikeyIndexEntitySingleParentDto(this MultikeyIndexEntitySingleParent projectFrom, IMapper mapper)
            => mapper.Map<MultikeyIndexEntitySingleParentDto>(projectFrom);

        public static List<MultikeyIndexEntitySingleParentDto> MapToMultikeyIndexEntitySingleParentDtoList(this IEnumerable<MultikeyIndexEntitySingleParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultikeyIndexEntitySingleParentDto(mapper)).ToList();
    }
}