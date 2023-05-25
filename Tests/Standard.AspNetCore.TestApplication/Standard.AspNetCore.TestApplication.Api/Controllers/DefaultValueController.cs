using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Api.Controllers
{
    [ApiController]
    [Route("api/default-value")]
    public class DefaultValueController : ControllerBase
    {
        private readonly IDefaultValueService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public DefaultValueController(IDefaultValueService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("operation-with-default-value")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OperationWithDefaultValue(
            string param1 = "Parameter 1 Value",
            CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.OperationWithDefaultValue(param1, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }
    }
}