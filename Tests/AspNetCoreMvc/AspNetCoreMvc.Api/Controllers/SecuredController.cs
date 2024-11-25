using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AspNetCoreMvc.Application.Common.Eventing;
using AspNetCoreMvc.Application.Interfaces;
using AspNetCoreMvc.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Mvc.MvcController", Version = "1.0")]

namespace AspNetCoreMvc.Api.Controllers
{
    [Authorize]
    public class SecuredController : Controller
    {
        private readonly ISecuredService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public SecuredController(ISecuredService appService, IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [HttpPost("operation")]
        public async Task<ActionResult> Operation(CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.Operation(cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return Ok();
        }
    }
}