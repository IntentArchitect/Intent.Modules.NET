using System;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Geometry
{
    public class GeometryDto
    {
        public GeometryDto()
        {
            Point = null!;
            MultiPolygon = null!;
        }

        public Guid Id { get; set; }
        public Point Point { get; set; }
        public MultiPolygon MultiPolygon { get; set; }

        public static GeometryDto Create(Guid id, Point point, MultiPolygon multiPolygon)
        {
            return new GeometryDto
            {
                Id = id,
                Point = point,
                MultiPolygon = multiPolygon
            };
        }
    }
}