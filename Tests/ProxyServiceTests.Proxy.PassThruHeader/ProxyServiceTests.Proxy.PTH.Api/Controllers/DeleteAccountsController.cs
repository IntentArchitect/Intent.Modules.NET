using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyServiceTests.Proxy.PTH.Application.Common.Interfaces;
using ProxyServiceTests.Proxy.PTH.Application.Common.Validation;
using ProxyServiceTests.Proxy.PTH.Application.Interfaces;
using ProxyServiceTests.Proxy.PTH.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Api.Controllers
{
    [ApiController]
    [Route("api/delete-accounts")]
    public class DeleteAccountsController : ControllerBase
    {
        private readonly IDeleteAccountsService _appService;
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountsController(IDeleteAccountsService appService,
            IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAccountCommand(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteAccountCommand(id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Ok();
        }
    }
}