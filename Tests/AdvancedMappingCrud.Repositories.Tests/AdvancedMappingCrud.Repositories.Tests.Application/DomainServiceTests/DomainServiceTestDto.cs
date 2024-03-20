using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests
{
    public class DomainServiceTestDto : IMapFrom<DomainServiceTest>
    {
        public DomainServiceTestDto()
        {
        }

        public Guid Id { get; set; }

        public static DomainServiceTestDto Create(Guid id)
        {
            return new DomainServiceTestDto
            {
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DomainServiceTest, DomainServiceTestDto>();
        }
    }
}