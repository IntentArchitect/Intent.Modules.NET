using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs
{
    public static class ExternalDocDtoMappingExtensions
    {
        public static ExternalDocDto MapToExternalDocDto(this ExternalDoc projectFrom, IMapper mapper)
            => mapper.Map<ExternalDocDto>(projectFrom);

        public static List<ExternalDocDto> MapToExternalDocDtoList(this IEnumerable<ExternalDoc> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToExternalDocDto(mapper)).ToList();
    }
}