using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.UniqueConVals
{
    public class UniqueConValDto : IMapFrom<UniqueConVal>
    {
        public UniqueConValDto()
        {
            Att1 = null!;
            Att2 = null!;
            AttInclude = null!;
        }

        public Guid Id { get; set; }
        public string Att1 { get; set; }
        public string Att2 { get; set; }
        public string AttInclude { get; set; }

        public static UniqueConValDto Create(Guid id, string att1, string att2, string attInclude)
        {
            return new UniqueConValDto
            {
                Id = id,
                Att1 = att1,
                Att2 = att2,
                AttInclude = attInclude
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UniqueConVal, UniqueConValDto>();
        }
    }
}