using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.ServiceToServiceInvocation;
using AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ServiceToServiceInvocation
{
    [ApiController]
    [Route("api/exposed-stored-proc")]
    public class ExposedStoredProcController : ControllerBase
    {
        private readonly IExposedStoredProcService _appService;
        private readonly IEventBus _eventBus;

        public ExposedStoredProcController(IExposedStoredProcService appService, IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;GetDataEntryDto&gt;.</response>
        [HttpGet("data")]
        [ProducesResponseType(typeof(List<GetDataEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetDataEntryDto>>> GetData(CancellationToken cancellationToken = default)
        {
            var result = default(List<GetDataEntryDto>);
            result = await _appService.GetData(cancellationToken);
            return Ok(result);
        }
    }
}