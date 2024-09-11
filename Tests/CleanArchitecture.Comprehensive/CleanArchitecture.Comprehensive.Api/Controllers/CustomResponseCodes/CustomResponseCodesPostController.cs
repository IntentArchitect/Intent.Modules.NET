using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse200;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse200WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse201;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse201WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse202;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse202WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse203;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse203WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse204;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse204WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse205;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse205WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse206;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse206WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse207;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse207WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse208;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse208WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse226;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse226WithResponse;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponseDefault;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponseDefaultWithResponse;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.Controllers.CustomResponseCodes
{
    [ApiController]
    public class CustomResponseCodesPostController : ControllerBase
    {
        private readonly ISender _mediator;

        public CustomResponseCodesPostController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response200")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse200(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse200(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response200-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse200WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse200WithResponse(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response201")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse201(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse201(), cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response201-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse201WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse201WithResponse(), cancellationToken);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="202">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response202")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse202(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse202(), cancellationToken);
            return Accepted();
        }

        /// <summary>
        /// </summary>
        /// <response code="202">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response202-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse202WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse202WithResponse(), cancellationToken);
            return Accepted(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="203">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response203")]
        [ProducesResponseType(StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse203(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse203(), cancellationToken);
            return StatusCode(203);
        }

        /// <summary>
        /// </summary>
        /// <response code="203">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response203-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status203NonAuthoritative)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse203WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse203WithResponse(), cancellationToken);
            return StatusCode(203, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response204")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse204(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse204(), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response204-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse204WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse204WithResponse(), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="205">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response205")]
        [ProducesResponseType(StatusCodes.Status205ResetContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse205(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse205(), cancellationToken);
            return StatusCode(205);
        }

        /// <summary>
        /// </summary>
        /// <response code="205">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response205-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status205ResetContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse205WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse205WithResponse(), cancellationToken);
            return StatusCode(205, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="206">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response206")]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse206(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse206(), cancellationToken);
            return StatusCode(206);
        }

        /// <summary>
        /// </summary>
        /// <response code="206">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response206-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse206WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse206WithResponse(), cancellationToken);
            return StatusCode(206, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="207">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response207")]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse207(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse207(), cancellationToken);
            return StatusCode(207);
        }

        /// <summary>
        /// </summary>
        /// <response code="207">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response207-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse207WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse207WithResponse(), cancellationToken);
            return StatusCode(207, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="208">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response208")]
        [ProducesResponseType(StatusCodes.Status208AlreadyReported)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse208(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse208(), cancellationToken);
            return StatusCode(208);
        }

        /// <summary>
        /// </summary>
        /// <response code="208">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response208-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status208AlreadyReported)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse208WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse208WithResponse(), cancellationToken);
            return StatusCode(208, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="226">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response226")]
        [ProducesResponseType(StatusCodes.Status226IMUsed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponse226(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponse226(), cancellationToken);
            return StatusCode(226);
        }

        /// <summary>
        /// </summary>
        /// <response code="226">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response226-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status226IMUsed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponse226WithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponse226WithResponse(), cancellationToken);
            return StatusCode(226, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response-default")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CustomResponseDefault(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CustomResponseDefault(), cancellationToken);
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("api/custom-response-codes/custom-response-default-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CustomResponseDefaultWithResponse(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CustomResponseDefaultWithResponse(), cancellationToken);
            return Created(string.Empty, result);
        }
    }
}