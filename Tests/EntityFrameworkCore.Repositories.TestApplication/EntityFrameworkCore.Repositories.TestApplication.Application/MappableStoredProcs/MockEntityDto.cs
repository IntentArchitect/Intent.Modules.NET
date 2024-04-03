using System;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Mappings;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public class MockEntityDto : IMapFrom<MockEntity>
    {
        public MockEntityDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static MockEntityDto Create(Guid id, string name)
        {
            return new MockEntityDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MockEntity, MockEntityDto>();
        }
    }
}