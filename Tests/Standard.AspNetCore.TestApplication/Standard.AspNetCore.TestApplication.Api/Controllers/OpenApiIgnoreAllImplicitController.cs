using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Standard.AspNetCore.TestApplication.Application;
using Standard.AspNetCore.TestApplication.Application.Common.Interfaces;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/ignore-all-implicit")]
    public class OpenApiIgnoreAllImplicitController : ControllerBase
    {
        private readonly IOpenApiIgnoreAllImplicitService _appService;
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public OpenApiIgnoreAllImplicitController(IOpenApiIgnoreAllImplicitService appService,
            IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("operation-a", Name = "OpenApiIgnoreAllImplicit.OperationA")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OperationA(CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.OperationA(cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Created(string.Empty, null);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        [HttpPost("operation-b")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OperationB(CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.OperationB(cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Created(string.Empty, null);
        }
    }
}