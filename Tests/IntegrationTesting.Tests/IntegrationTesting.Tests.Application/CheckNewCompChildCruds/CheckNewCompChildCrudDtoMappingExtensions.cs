using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds
{
    public static class CheckNewCompChildCrudDtoMappingExtensions
    {
        public static CheckNewCompChildCrudDto MapToCheckNewCompChildCrudDto(this CheckNewCompChildCrud projectFrom, IMapper mapper)
            => mapper.Map<CheckNewCompChildCrudDto>(projectFrom);

        public static List<CheckNewCompChildCrudDto> MapToCheckNewCompChildCrudDtoList(this IEnumerable<CheckNewCompChildCrud> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCheckNewCompChildCrudDto(mapper)).ToList();
    }
}