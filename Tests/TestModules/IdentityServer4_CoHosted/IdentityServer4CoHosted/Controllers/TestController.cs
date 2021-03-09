using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4CoHosted.Contracts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace IdentityServer4CoHosted.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _appService;

        public TestController(ITestService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [HttpPost("anonymous")]
        public async Task<ActionResult> AnonymousOperation()
        {

            await _appService.AnonymousOperation();

            return NoContent();
        }

        [HttpPost("authenticated")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AuthenticatedOperation()
        {

            await _appService.AuthenticatedOperation();

            return NoContent();
        }

        [HttpPost("authorized")]
        [Authorize(Roles = "MyRole", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AuthorizedOperation()
        {

            await _appService.AuthorizedOperation();

            return NoContent();
        }


    }
}