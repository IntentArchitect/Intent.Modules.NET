using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    public static class MultikeyIndexEntityMultiChildDtoMappingExtensions
    {
        public static MultikeyIndexEntityMultiChildDto MapToMultikeyIndexEntityMultiChildDto(this MultikeyIndexEntityMultiChild projectFrom, IMapper mapper)
            => mapper.Map<MultikeyIndexEntityMultiChildDto>(projectFrom);

        public static List<MultikeyIndexEntityMultiChildDto> MapToMultikeyIndexEntityMultiChildDtoList(this IEnumerable<MultikeyIndexEntityMultiChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultikeyIndexEntityMultiChildDto(mapper)).ToList();
    }
}