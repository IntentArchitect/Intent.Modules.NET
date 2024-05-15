using System;
using System.Collections.Generic;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Common.Mappings;
using AzureFunctions.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
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