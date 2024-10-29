using System;
using System.Collections.Generic;
using AutoMapper;
using AzureFunctions.NET8.Application.Common.Mappings;
using AzureFunctions.NET8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.NET8.Application.SampleDomains
{
    public class SampleDomainDto : IMapFrom<SampleDomain>
    {
        public SampleDomainDto()
        {
            Attribute = null!;
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; }
        public string Name { get; set; }

        public static SampleDomainDto Create(Guid id, string attribute, string name)
        {
            return new SampleDomainDto
            {
                Id = id,
                Attribute = attribute,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SampleDomain, SampleDomainDto>();
        }
    }
}