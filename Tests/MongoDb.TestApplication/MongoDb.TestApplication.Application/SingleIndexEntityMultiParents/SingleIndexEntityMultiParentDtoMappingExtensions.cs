using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    public static class SingleIndexEntityMultiParentDtoMappingExtensions
    {
        public static SingleIndexEntityMultiParentDto MapToSingleIndexEntityMultiParentDto(this SingleIndexEntityMultiParent projectFrom, IMapper mapper)
            => mapper.Map<SingleIndexEntityMultiParentDto>(projectFrom);

        public static List<SingleIndexEntityMultiParentDto> MapToSingleIndexEntityMultiParentDtoList(this IEnumerable<SingleIndexEntityMultiParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSingleIndexEntityMultiParentDto(mapper)).ToList();
    }
}