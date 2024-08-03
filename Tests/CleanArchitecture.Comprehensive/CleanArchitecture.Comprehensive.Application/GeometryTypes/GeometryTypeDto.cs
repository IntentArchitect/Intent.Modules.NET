using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Geometry;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes
{
    public class GeometryTypeDto : IMapFrom<GeometryType>
    {
        public GeometryTypeDto()
        {
            Point = null!;
        }

        public Guid Id { get; set; }
        public Point Point { get; set; }

        public static GeometryTypeDto Create(Guid id, Point point)
        {
            return new GeometryTypeDto
            {
                Id = id,
                Point = point
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GeometryType, GeometryTypeDto>();
        }
    }
}