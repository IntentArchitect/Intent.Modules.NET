using System;
using System.Collections.Generic;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Common.Mappings;
using AzureFunctions.TestApplication.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
{

    public class SampleDomainDTO : IMapFrom<SampleDomain>
    {
        public SampleDomainDTO()
        {
        }

        public static SampleDomainDTO Create(
            Guid id)
        {
            return new SampleDomainDTO
            {
                Id = id,
            };
        }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SampleDomain, SampleDomainDTO>();
        }
    }
}