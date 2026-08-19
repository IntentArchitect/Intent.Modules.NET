using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Geometry;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Interfaces.Geometry
{
    public interface IGeometryService
    {
        Task<List<GeometryDto>> GetGeometryTypes(CancellationToken cancellationToken = default);
    }
}