using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public static class MockEntityDtoMappingExtensions
    {
        public static MockEntityDto MapToMockEntityDto(this MockEntity projectFrom, IMapper mapper)
            => mapper.Map<MockEntityDto>(projectFrom);

        public static List<MockEntityDto> MapToMockEntityDtoList(this IEnumerable<MockEntity> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMockEntityDto(mapper)).ToList();
    }
}