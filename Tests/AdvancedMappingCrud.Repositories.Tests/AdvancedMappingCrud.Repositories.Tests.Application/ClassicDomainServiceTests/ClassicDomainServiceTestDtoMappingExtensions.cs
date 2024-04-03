using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests
{
    public static class ClassicDomainServiceTestDtoMappingExtensions
    {
        public static ClassicDomainServiceTestDto MapToClassicDomainServiceTestDto(this ClassicDomainServiceTest projectFrom, IMapper mapper)
            => mapper.Map<ClassicDomainServiceTestDto>(projectFrom);

        public static List<ClassicDomainServiceTestDto> MapToClassicDomainServiceTestDtoList(this IEnumerable<ClassicDomainServiceTest> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClassicDomainServiceTestDto(mapper)).ToList();
    }
}