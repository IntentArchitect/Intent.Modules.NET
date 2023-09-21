using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.DDD
{
    public static class GetCameraDtoMappingExtensions
    {
        public static GetCameraDto MapToGetCameraDto(this Camera projectFrom, IMapper mapper)
            => mapper.Map<GetCameraDto>(projectFrom);

        public static List<GetCameraDto> MapToGetCameraDtoList(this IEnumerable<Camera> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGetCameraDto(mapper)).ToList();
    }
}