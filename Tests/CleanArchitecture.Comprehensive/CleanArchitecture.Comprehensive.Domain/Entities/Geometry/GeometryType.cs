using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.Geometry
{
    public class GeometryType : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Point Point { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}