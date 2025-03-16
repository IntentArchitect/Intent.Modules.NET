using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Application.OperationMapping;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers.OperationMapping
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _appService;
        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public UserController(IUserService appService,
            IValidationService validationService,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("with-task-item")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUserWithTaskItemService(
            string userName,
            string listName,
            string taskName,
            [FromBody] List<CreateUserWithTaskItemServiceSubTasksDto> subTasks,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(subTasks, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.CreateUserWithTaskItemService(userName, listName, taskName, subTasks, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Created(string.Empty, null);
        }
    }
}