using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures
{
    public static class BadSignaturesDtoMappingExtensions
    {
        public static BadSignaturesDto MapToBadSignaturesDto(this Domain.Entities.BadSignatures projectFrom, IMapper mapper)
            => mapper.Map<BadSignaturesDto>(projectFrom);

        public static List<BadSignaturesDto> MapToBadSignaturesDtoList(this IEnumerable<Domain.Entities.BadSignatures> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBadSignaturesDto(mapper)).ToList();
    }
}