using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos
{
    public static class SpResultDtoMappingExtensions
    {
        public static SpResultDto MapToSpResultDto(this SpResult projectFrom, IMapper mapper)
            => mapper.Map<SpResultDto>(projectFrom);

        public static List<SpResultDto> MapToSpResultDtoList(this IEnumerable<SpResult> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSpResultDto(mapper)).ToList();
    }
}