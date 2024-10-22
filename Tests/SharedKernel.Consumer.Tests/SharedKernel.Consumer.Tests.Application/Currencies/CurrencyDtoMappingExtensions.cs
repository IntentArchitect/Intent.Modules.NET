using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Kernel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Currencies
{
    public static class CurrencyDtoMappingExtensions
    {
        public static CurrencyDto MapToCurrencyDto(this Currency projectFrom, IMapper mapper)
            => mapper.Map<CurrencyDto>(projectFrom);

        public static List<CurrencyDto> MapToCurrencyDtoList(this IEnumerable<Currency> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCurrencyDto(mapper)).ToList();
    }
}