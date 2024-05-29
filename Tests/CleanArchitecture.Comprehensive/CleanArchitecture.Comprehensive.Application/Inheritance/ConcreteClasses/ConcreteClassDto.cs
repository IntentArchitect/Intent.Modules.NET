using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses
{
    public class ConcreteClassDto : IMapFrom<ConcreteClass>
    {
        public ConcreteClassDto()
        {
            ConcreteAttr = null!;
            BaseAttr = null!;
        }

        public Guid Id { get; set; }
        public string ConcreteAttr { get; set; }
        public string BaseAttr { get; set; }

        public static ConcreteClassDto Create(Guid id, string concreteAttr, string baseAttr)
        {
            return new ConcreteClassDto
            {
                Id = id,
                ConcreteAttr = concreteAttr,
                BaseAttr = baseAttr
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ConcreteClass, ConcreteClassDto>();
        }
    }
}