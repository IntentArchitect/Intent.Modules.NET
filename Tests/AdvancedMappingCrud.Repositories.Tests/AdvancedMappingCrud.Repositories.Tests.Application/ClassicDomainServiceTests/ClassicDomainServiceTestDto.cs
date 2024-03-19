using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests
{
    public class ClassicDomainServiceTestDto : IMapFrom<ClassicDomainServiceTest>
    {
        public ClassicDomainServiceTestDto()
        {
        }

        public Guid Id { get; set; }

        public static ClassicDomainServiceTestDto Create(Guid id)
        {
            return new ClassicDomainServiceTestDto
            {
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ClassicDomainServiceTest, ClassicDomainServiceTestDto>();
        }
    }
}