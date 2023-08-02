using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.WithCompositeKeys
{
    public static class WithCompositeKeyDtoMappingExtensions
    {
        public static WithCompositeKeyDto MapToWithCompositeKeyDto(this WithCompositeKey projectFrom, IMapper mapper)
            => mapper.Map<WithCompositeKeyDto>(projectFrom);

        public static List<WithCompositeKeyDto> MapToWithCompositeKeyDtoList(this IEnumerable<WithCompositeKey> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToWithCompositeKeyDto(mapper)).ToList();
    }
}