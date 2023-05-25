using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CleanArchitecture.TestApplication.Application.Interfaces.ServiceDispatch;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Api.Controllers.ServiceDispatch
{
    [ApiController]
    [Route("api/service-dispatch")]
    public class ServiceDispatchController : ControllerBase
    {
        private readonly IServiceDispatchService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDispatchController(IServiceDispatchService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("mutation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Mutation(CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                _appService.Mutation();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("mutation-with-param")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Mutation(string param, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                _appService.Mutation(param);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("mutation-async")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MutationAsync(CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.MutationAsync(cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("mutation-async-with-param")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MutationAsync(string param, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.MutationAsync(param, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("query-with-param")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> Query(string param, CancellationToken cancellationToken = default)
        {
            var result = default(string);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = _appService.Query(param);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("query")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> Query(CancellationToken cancellationToken = default)
        {
            var result = default(string);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = _appService.Query();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("query-with-param")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> QueryAsync(CancellationToken cancellationToken = default)
        {
            var result = default(string);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.QueryAsync(cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("query-async-with-param")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> QueryAsync(string param, CancellationToken cancellationToken = default)
        {
            var result = default(string);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.QueryAsync(param, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, result);
        }
    }
}