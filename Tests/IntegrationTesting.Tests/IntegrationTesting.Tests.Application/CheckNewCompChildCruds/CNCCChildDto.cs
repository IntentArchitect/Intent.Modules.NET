using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds
{
    public class CNCCChildDto : IMapFrom<CNCCChild>
    {
        public CNCCChildDto()
        {
            Description = null!;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }

        public static CNCCChildDto Create(Guid checkNewCompChildCrudId, Guid id, string description)
        {
            return new CNCCChildDto
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId,
                Id = id,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CNCCChild, CNCCChildDto>();
        }
    }
}