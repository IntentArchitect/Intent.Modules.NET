using System;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Geometry
{
    public class GeometryType
    {
        public Guid Id { get; set; }

        public Point Point { get; set; }
    }
}