using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests
{
    public static class DomainServiceTestDtoMappingExtensions
    {
        public static DomainServiceTestDto MapToDomainServiceTestDto(this DomainServiceTest projectFrom, IMapper mapper)
            => mapper.Map<DomainServiceTestDto>(projectFrom);

        public static List<DomainServiceTestDto> MapToDomainServiceTestDtoList(this IEnumerable<DomainServiceTest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToDomainServiceTestDto(mapper)).ToList();
    }
}