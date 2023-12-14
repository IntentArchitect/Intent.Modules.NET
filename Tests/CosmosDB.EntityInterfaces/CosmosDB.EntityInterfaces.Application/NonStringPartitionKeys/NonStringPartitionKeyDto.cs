using AutoMapper;
using CosmosDB.EntityInterfaces.Application.Common.Mappings;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.NonStringPartitionKeys
{
    public class NonStringPartitionKeyDto : IMapFrom<NonStringPartitionKey>
    {
        public NonStringPartitionKeyDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public int PartInt { get; set; }
        public string Name { get; set; }

        public static NonStringPartitionKeyDto Create(string id, int partInt, string name)
        {
            return new NonStringPartitionKeyDto
            {
                Id = id,
                PartInt = partInt,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NonStringPartitionKey, NonStringPartitionKeyDto>();
        }
    }
}