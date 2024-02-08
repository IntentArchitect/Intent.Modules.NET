using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Children
{
    public class ChildDto : IMapFrom<Child>
    {
        public ChildDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MyParentId { get; set; }

        public static ChildDto Create(Guid id, string name, Guid myParentId)
        {
            return new ChildDto
            {
                Id = id,
                Name = name,
                MyParentId = myParentId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Child, ChildDto>();
        }
    }
}