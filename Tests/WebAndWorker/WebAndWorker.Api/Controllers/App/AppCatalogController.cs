using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAndWorker.Application.Common.Eventing;
using WebAndWorker.Application.Interfaces.App;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace WebAndWorker.Api.Controllers.App
{
    [ApiController]
    public class AppCatalogController : ControllerBase
    {
        private readonly IAppCatalogService _appService;
        private readonly IMessageBus _messageBus;

        public AppCatalogController(IAppCatalogService appService, IMessageBus messageBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified string.</response>
        [HttpGet("api/app/items")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetAppItems(CancellationToken cancellationToken = default)
        {
            var result = default(string);
            result = await _appService.GetAppItems(cancellationToken);
            return Ok(result);
        }
    }
}