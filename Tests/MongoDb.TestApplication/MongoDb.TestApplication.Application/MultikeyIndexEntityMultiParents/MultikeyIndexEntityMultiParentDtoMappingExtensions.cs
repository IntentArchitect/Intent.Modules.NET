using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    public static class MultikeyIndexEntityMultiParentDtoMappingExtensions
    {
        public static MultikeyIndexEntityMultiParentDto MapToMultikeyIndexEntityMultiParentDto(this MultikeyIndexEntityMultiParent projectFrom, IMapper mapper)
            => mapper.Map<MultikeyIndexEntityMultiParentDto>(projectFrom);

        public static List<MultikeyIndexEntityMultiParentDto> MapToMultikeyIndexEntityMultiParentDtoList(this IEnumerable<MultikeyIndexEntityMultiParent> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMultikeyIndexEntityMultiParentDto(mapper)).ToList();
    }
}