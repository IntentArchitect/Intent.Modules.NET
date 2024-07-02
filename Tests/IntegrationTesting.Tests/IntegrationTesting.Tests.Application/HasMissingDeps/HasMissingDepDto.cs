using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps
{
    public class HasMissingDepDto : IMapFrom<HasMissingDep>
    {
        public HasMissingDepDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MissingDepId { get; set; }

        public static HasMissingDepDto Create(Guid id, string name, Guid missingDepId)
        {
            return new HasMissingDepDto
            {
                Id = id,
                Name = name,
                MissingDepId = missingDepId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<HasMissingDep, HasMissingDepDto>();
        }
    }
}