using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAndWorker.Application.Common.Eventing;
using WebAndWorker.Application.Interfaces.Mobile;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace WebAndWorker.Mobile.Api.Controllers.Mobile
{
    [ApiController]
    public class MobileCatalogController : ControllerBase
    {
        private readonly IMobileCatalogService _appService;
        private readonly IMessageBus _messageBus;

        public MobileCatalogController(IMobileCatalogService appService, IMessageBus messageBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified string.</response>
        [HttpGet("api/mobile/items")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetMobileItems(CancellationToken cancellationToken = default)
        {
            var result = default(string);
            result = await _appService.GetMobileItems(cancellationToken);
            return Ok(result);
        }
    }
}