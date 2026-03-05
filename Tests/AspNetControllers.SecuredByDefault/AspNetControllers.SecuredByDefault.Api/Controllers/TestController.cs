using AspNetControllers.SecuredByDefault.Application.Common.Interfaces;
using AspNetControllers.SecuredByDefault.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AspNetControllers.SecuredByDefault.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _appService;
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;

        public TestController(ITestService appService, IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        [HttpPost("operation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Operation(CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                await _appService.Operation(cancellationToken);
                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Created(string.Empty, null);
        }
    }
}