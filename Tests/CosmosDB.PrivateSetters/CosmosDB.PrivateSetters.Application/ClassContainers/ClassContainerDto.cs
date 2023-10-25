using AutoMapper;
using CosmosDB.PrivateSetters.Application.Common.Mappings;
using CosmosDB.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers
{
    public class ClassContainerDto : IMapFrom<ClassContainer>
    {
        public ClassContainerDto()
        {
            Id = null!;
            ClassPartitionKey = null!;
        }

        public string Id { get; set; }
        public string ClassPartitionKey { get; set; }

        public static ClassContainerDto Create(string id, string classPartitionKey)
        {
            return new ClassContainerDto
            {
                Id = id,
                ClassPartitionKey = classPartitionKey
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ClassContainer, ClassContainerDto>();
        }
    }
}