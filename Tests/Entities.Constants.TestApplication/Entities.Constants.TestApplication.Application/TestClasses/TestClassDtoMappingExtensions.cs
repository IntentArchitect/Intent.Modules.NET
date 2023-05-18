using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Entities.Constants.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses
{
    public static class TestClassDtoMappingExtensions
    {
        public static TestClassDto MapToTestClassDto(this TestClass projectFrom, IMapper mapper)
        {
            return mapper.Map<TestClassDto>(projectFrom);
        }

        public static List<TestClassDto> MapToTestClassDtoList(this IEnumerable<TestClass> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToTestClassDto(mapper)).ToList();
        }
    }
}