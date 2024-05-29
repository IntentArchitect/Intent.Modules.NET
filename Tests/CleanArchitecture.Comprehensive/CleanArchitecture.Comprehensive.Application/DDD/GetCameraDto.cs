using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD
{
    public class GetCameraDto : IMapFrom<Camera>
    {
        public GetCameraDto()
        {
            IdemiaId = null!;
        }

        public Guid Id { get; set; }
        public string IdemiaId { get; set; }

        public static GetCameraDto Create(Guid id, string idemiaId)
        {
            return new GetCameraDto
            {
                Id = id,
                IdemiaId = idemiaId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Camera, GetCameraDto>();
        }
    }
}