using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Application.Common.Mappings;
using ValueObjects.Record.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public class TestEntityDto : IMapFrom<TestEntity>
    {
        public TestEntityDto()
        {
            Name = null!;
            Amount = null!;
            Address = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public TestEntityMoneyDto Amount { get; set; }
        public TestEntityAddressDto Address { get; set; }

        public static TestEntityDto Create(Guid id, string name, TestEntityMoneyDto amount, TestEntityAddressDto address)
        {
            return new TestEntityDto
            {
                Id = id,
                Name = name,
                Amount = amount,
                Address = address
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TestEntity, TestEntityDto>();
        }
    }
}