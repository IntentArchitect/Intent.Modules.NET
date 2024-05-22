using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities
{
    public static class TestEntityAddressDtoMappingExtensions
    {
        public static TestEntityAddressDto MapToTestEntityAddressDto(this Address projectFrom, IMapper mapper)
            => mapper.Map<TestEntityAddressDto>(projectFrom);

        public static List<TestEntityAddressDto> MapToTestEntityAddressDtoList(this IEnumerable<Address> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestEntityAddressDto(mapper)).ToList();
    }
}