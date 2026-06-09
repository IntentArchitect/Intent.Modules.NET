using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.AzureServiceBus.Application.Common.Eventing;
using NServiceBus.AzureServiceBus.Application.Interfaces.Animals;
using NServiceBus.AzureServiceBus.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Api.Controllers.Animals
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalsService _appService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;

        public AnimalsController(IAnimalsService appService, IUnitOfWork unitOfWork, IMessageBus messageBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateAnimal(string dto, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.CreateAnimal(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _messageBus.FlushAllAsync(cancellationToken);
            return Created(string.Empty, null);
        }
    }
}