using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches
{
    public static class CheckAddressDCDtoMappingExtensions
    {
        public static CheckAddressDCDto MapToCheckAddressDCDto(this AddressDC projectFrom, IMapper mapper)
            => mapper.Map<CheckAddressDCDto>(projectFrom);

        public static List<CheckAddressDCDto> MapToCheckAddressDCDtoList(this IEnumerable<AddressDC> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCheckAddressDCDto(mapper)).ToList();
    }
}