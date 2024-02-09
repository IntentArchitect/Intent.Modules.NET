using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.NoReturns
{
    public static class NoReturnDtoMappingExtensions
    {
        public static NoReturnDto MapToNoReturnDto(this NoReturn projectFrom, IMapper mapper)
            => mapper.Map<NoReturnDto>(projectFrom);

        public static List<NoReturnDto> MapToNoReturnDtoList(this IEnumerable<NoReturn> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToNoReturnDto(mapper)).ToList();
    }
}