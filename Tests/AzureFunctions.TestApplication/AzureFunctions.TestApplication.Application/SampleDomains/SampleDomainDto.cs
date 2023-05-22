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
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; } = null!;

        public static SampleDomainDto Create(Guid id, string attribute)
        {
            return new SampleDomainDto
            {
                Id = id,
                Attribute = attribute
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SampleDomain, SampleDomainDto>();
        }
    }
}