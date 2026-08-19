using System;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Entities
{
    public class GeometryType
    {
        public GeometryType()
        {
            Point = null!;
            MultiPolygon = null!;
        }

        public Guid Id { get; set; }

        public Point Point { get; set; }

        public MultiPolygon MultiPolygon { get; set; }
    }
}