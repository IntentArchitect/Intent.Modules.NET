using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures
{
    public class BadSignaturesDto : IMapFrom<Domain.Entities.BadSignatures>
    {
        public BadSignaturesDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static BadSignaturesDto Create(Guid id, string name)
        {
            return new BadSignaturesDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.BadSignatures, BadSignaturesDto>();
        }
    }
}