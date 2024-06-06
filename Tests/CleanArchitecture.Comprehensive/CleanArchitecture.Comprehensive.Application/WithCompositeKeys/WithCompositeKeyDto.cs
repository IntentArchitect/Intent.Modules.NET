using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys
{
    public class WithCompositeKeyDto : IMapFrom<WithCompositeKey>
    {
        public WithCompositeKeyDto()
        {
            Name = null!;
        }

        public Guid Key1Id { get; set; }
        public Guid Key2Id { get; set; }
        public string Name { get; set; }

        public static WithCompositeKeyDto Create(Guid key1Id, Guid key2Id, string name)
        {
            return new WithCompositeKeyDto
            {
                Key1Id = key1Id,
                Key2Id = key2Id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<WithCompositeKey, WithCompositeKeyDto>();
        }
    }
}