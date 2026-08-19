using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Geometry;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Interfaces.Geometry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Implementation.Geometry
{
    [IntentManaged(Mode.Merge)]
    public class GeometryService : IGeometryService
    {
        private readonly ILogger<GeometryService> _logger;

        [IntentManaged(Mode.Merge)]
        public GeometryService(ILogger<GeometryService> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<GeometryDto>> GetGeometryTypes(CancellationToken cancellationToken = default)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var point = geometryFactory.CreatePoint(new Coordinate(-73.935242, 40.730610));

            var polygon1 = geometryFactory.CreatePolygon(new[]
            {
                new Coordinate(-73.9857, 40.7484),
                new Coordinate(-73.9857, 40.7584),
                new Coordinate(-73.9757, 40.7584),
                new Coordinate(-73.9757, 40.7484),
                new Coordinate(-73.9857, 40.7484)
            });

            var polygon2 = geometryFactory.CreatePolygon(new[]
            {
                new Coordinate(-74.0060, 40.7128),
                new Coordinate(-74.0060, 40.7228),
                new Coordinate(-73.9960, 40.7228),
                new Coordinate(-73.9960, 40.7128),
                new Coordinate(-74.0060, 40.7128)
            });

            var multiPolygon = geometryFactory.CreateMultiPolygon(new[] { polygon1, polygon2 });

            var result = new List<GeometryDto>
            {
                GeometryDto.Create(Guid.NewGuid(), point, multiPolygon)
            };

            _logger.LogInformation(
                "GetGeometryTypes response payload: {@Result}",
                result);

            return await Task.FromResult(result);
        }
    }
}