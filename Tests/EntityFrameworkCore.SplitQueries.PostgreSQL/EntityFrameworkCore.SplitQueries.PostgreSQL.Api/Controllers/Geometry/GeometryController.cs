using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Geometry;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Application.Interfaces.Geometry;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Api.Controllers.Geometry
{
    [ApiController]
    public class GeometryController : ControllerBase
    {
        private readonly IGeometryService _appService;
        public GeometryController(IGeometryService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;GeometryDto&gt;.</response>
        [HttpGet("api/geometry-types")]
        [ProducesResponseType(typeof(List<GeometryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GeometryDto>>> GetGeometryTypes(CancellationToken cancellationToken = default)
        {
            var result = default(List<GeometryDto>);
            result = await _appService.GetGeometryTypes(cancellationToken);
            return Ok(result);
        }
    }
}
