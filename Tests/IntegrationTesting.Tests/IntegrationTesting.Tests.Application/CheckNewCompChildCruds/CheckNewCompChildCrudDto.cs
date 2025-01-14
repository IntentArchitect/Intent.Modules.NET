using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds
{
    public class CheckNewCompChildCrudDto : IMapFrom<CheckNewCompChildCrud>
    {
        public CheckNewCompChildCrudDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CheckNewCompChildCrudDto Create(Guid id, string name)
        {
            return new CheckNewCompChildCrudDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CheckNewCompChildCrud, CheckNewCompChildCrudDto>();
        }
    }
}