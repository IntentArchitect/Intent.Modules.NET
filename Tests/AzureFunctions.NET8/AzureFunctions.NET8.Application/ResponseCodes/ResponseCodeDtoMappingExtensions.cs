using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AzureFunctions.NET8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes
{
    public static class ResponseCodeDtoMappingExtensions
    {
        public static ResponseCodeDto MapToResponseCodeDto(this ResponseCode projectFrom, IMapper mapper)
            => mapper.Map<ResponseCodeDto>(projectFrom);

        public static List<ResponseCodeDto> MapToResponseCodeDtoList(this IEnumerable<ResponseCode> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToResponseCodeDto(mapper)).ToList();
    }
}