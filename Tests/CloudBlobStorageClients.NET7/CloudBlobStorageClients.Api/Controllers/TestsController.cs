using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CloudBlobStorageClients.Application.Tests.TestAwsS3;
using CloudBlobStorageClients.Application.Tests.TestAzure;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CloudBlobStorageClients.Api.Controllers
{
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ISender _mediator;

        public TestsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/tests/test-aws-s3")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TestAwsS3(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new TestAwsS3Command(), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        [HttpPut("api/tests/test-azure")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TestAzure(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new TestAzureCommand(), cancellationToken);
            return NoContent();
        }
    }
}