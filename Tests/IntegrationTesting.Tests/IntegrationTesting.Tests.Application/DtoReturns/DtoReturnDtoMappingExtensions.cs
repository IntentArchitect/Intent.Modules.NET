using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns
{
    public static class DtoReturnDtoMappingExtensions
    {
        public static DtoReturnDto MapToDtoReturnDto(this DtoReturn projectFrom, IMapper mapper)
            => mapper.Map<DtoReturnDto>(projectFrom);

        public static List<DtoReturnDto> MapToDtoReturnDtoList(this IEnumerable<DtoReturn> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDtoReturnDto(mapper)).ToList();
    }
}