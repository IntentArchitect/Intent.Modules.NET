using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDb.TestApplication.Api.Controllers.ResponseTypes;
using MongoDb.TestApplication.Application.Common.Validation;
using MongoDb.TestApplication.Application.IdTypeInts;
using MongoDb.TestApplication.Application.Interfaces;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace MongoDb.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdTypeIntsController : ControllerBase
    {
        private readonly IIdTypeIntsService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationService _validationService;
        private readonly IMongoDbUnitOfWork _mongoDbUnitOfWork;

        public IdTypeIntsController(IIdTypeIntsService appService,
            IUnitOfWork unitOfWork,
            IValidationService validationService,
            IMongoDbUnitOfWork mongoDbUnitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _validationService = validationService;
            _mongoDbUnitOfWork = mongoDbUnitOfWork ?? throw new ArgumentNullException(nameof(mongoDbUnitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> Create([FromBody] IdTypeIntCreateDto dto, CancellationToken cancellationToken)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(int);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Create(dto);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Created(string.Empty, new JsonResponse<int>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified IdTypeIntDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">Can't find an IdTypeIntDto with the parameters provided.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IdTypeIntDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IdTypeIntDto>> FindById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = default(IdTypeIntDto);
            result = await _appService.FindById(id);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;IdTypeIntDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<IdTypeIntDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<IdTypeIntDto>>> FindAll(CancellationToken cancellationToken)
        {
            var result = default(List<IdTypeIntDto>);
            result = await _appService.FindAll();
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] IdTypeIntUpdateDto dto, CancellationToken cancellationToken)
        {
            await _validationService.Handle(dto, cancellationToken);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.Put(id, dto);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IdTypeIntDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IdTypeIntDto>> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = default(IdTypeIntDto);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.Delete(id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);
            return Ok(result);
        }
    }
}