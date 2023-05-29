using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    public static class SingleIndexEntityMultiChildDtoMappingExtensions
    {
        public static SingleIndexEntityMultiChildDto MapToSingleIndexEntityMultiChildDto(this SingleIndexEntityMultiChild projectFrom, IMapper mapper)
            => mapper.Map<SingleIndexEntityMultiChildDto>(projectFrom);

        public static List<SingleIndexEntityMultiChildDto> MapToSingleIndexEntityMultiChildDtoList(this IEnumerable<SingleIndexEntityMultiChild> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSingleIndexEntityMultiChildDto(mapper)).ToList();
    }
}