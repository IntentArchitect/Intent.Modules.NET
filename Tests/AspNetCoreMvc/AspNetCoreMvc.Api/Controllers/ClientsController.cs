using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AspNetCoreMvc.Application.ClientsService;
using AspNetCoreMvc.Application.Common.Eventing;
using AspNetCoreMvc.Application.Interfaces;
using AspNetCoreMvc.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Mvc.MvcController", Version = "1.0")]

namespace AspNetCoreMvc.Api.Controllers
{
    [Route("clients/{groupId}")]
    public class ClientsController : Controller
    {
        private readonly IClientsService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public ClientsController(IClientsService appService, IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [HttpPost]
        public async Task<ActionResult> CreateClient(
            string groupId,
            ClientCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = Guid.Empty;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateClient(groupId, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClientById", new { id = result, groupId });
        }

        [IntentIgnore]
        [HttpGet("create")]
        public ActionResult CreateNew(string groupId)
        {
            return View();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindClientById(
            string groupId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(ClientDto);
            result = await _appService.FindClientById(groupId, id, cancellationToken);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> FindClients(string groupId, CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.FindClients(groupId, cancellationToken);

            return View(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateClient(
            string groupId,
            Guid id,
            ClientUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateClient(groupId, id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClients", new { groupId });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(string groupId, Guid id, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteClient(groupId, id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClients", new { groupId });
        }

        [HttpGet("without-view")]
        public async Task<ActionResult<List<ClientDto>>> FindClientsWithoutView(
            string groupId,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.FindClientsWithoutView(groupId, cancellationToken);

            return result;
        }
    }
}