using System.Net.Mime;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Api.Controllers.ResponseTypes;
using JsonPatchRfc7396.Scalar.Api.Patching;
using JsonPatchRfc7396.Scalar.Application.Common.Validation;
using JsonPatchRfc7396.Scalar.Application.Documents;
using JsonPatchRfc7396.Scalar.Application.Documents.CreateDocument;
using JsonPatchRfc7396.Scalar.Application.Documents.DeleteDocument;
using JsonPatchRfc7396.Scalar.Application.Documents.GetDocumentById;
using JsonPatchRfc7396.Scalar.Application.Documents.GetDocuments;
using JsonPatchRfc7396.Scalar.Application.Documents.PatchDocument;
using JsonPatchRfc7396.Scalar.Application.Documents.UpdateDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Morcatko.AspNetCore.JsonMergePatch;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Api.Controllers
{
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IValidatorProvider _validatorProvider;

        public DocumentsController(ISender mediator, IValidatorProvider validatorProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("api/documents")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<string>>> CreateDocument(
            [FromBody] CreateDocumentCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetDocumentById), new { id = result }, new JsonResponse<string>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("api/documents/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDocument(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteDocumentCommand(id: id), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPatch("api/documents/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Consumes(JsonMergePatchDocument.ContentType)]
        public async Task<ActionResult> PatchDocument(
            [FromRoute] string id,
            [FromBody] JsonMergePatchDocument<PatchDocumentCommand> mergePatchDocument,
            CancellationToken cancellationToken = default)
        {
            if (mergePatchDocument == null)
            {
                return BadRequest("Merge patch document cannot be null");
            }
            var patchExecutor = new JsonMergePatchExecutor<PatchDocumentCommand>(mergePatchDocument, _validatorProvider);
            var command = new PatchDocumentCommand(id, patchExecutor);

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("api/documents/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDocument(
            [FromRoute] string id,
            [FromBody] UpdateDocumentCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == default)
            {
                command.Id = id;
            }

            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified DocumentDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No DocumentDto could be found with the provided parameters.</response>
        [HttpGet("api/documents/{id}")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentDto>> GetDocumentById(
            [FromRoute] string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDocumentByIdQuery(id: id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;DocumentDto&gt;.</response>
        [HttpGet("api/documents")]
        [ProducesResponseType(typeof(List<DocumentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DocumentDto>>> GetDocuments(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDocumentsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}