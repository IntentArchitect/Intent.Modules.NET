using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AspNetCoreMvc.Application.Clients;
using AspNetCoreMvc.Application.Common.Eventing;
using AspNetCoreMvc.Application.Interfaces;
using AspNetCoreMvc.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Mvc.MvcController", Version = "1.0")]

namespace AspNetCoreMvc.Api.Controllers
{
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
        public async Task<ActionResult> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default)
        {
            var result = Guid.Empty;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateClient(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClients", result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindClientById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = default(ClientDto);
            result = await _appService.FindClientById(id, cancellationToken);

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> FindClients(CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.FindClients(cancellationToken);

            return View(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateClient(
            Guid id,
            ClientUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.UpdateClient(id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClients");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(Guid id, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteClient(id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);

            return RedirectToAction("FindClients");
        }

        [HttpGet("without-view")]
        public async Task<ActionResult> FindClientsWithoutView(CancellationToken cancellationToken = default)
        {
            var result = default(List<ClientDto>);
            result = await _appService.FindClientsWithoutView(cancellationToken);

            return result;
        }
    }
}