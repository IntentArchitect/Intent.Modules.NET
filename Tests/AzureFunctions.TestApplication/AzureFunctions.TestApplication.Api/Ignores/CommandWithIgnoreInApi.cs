using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.TestApplication.Application.Ignores.CommandWithIgnoreInApi;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.Ignores
{
    public class CommandWithIgnoreInApi
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CommandWithIgnoreInApi(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("Ignores_CommandWithIgnoreInApi")]
        [OpenApiOperation("CommandWithIgnoreInApi", tags: new[] { "Ignores" }, Description = "Command with ignore in api")]
        [OpenApiIgnoreAttribute]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "ignores/command-with-ignore-in-api")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new Application.Ignores.CommandWithIgnoreInApi.CommandWithIgnoreInApi(), cancellationToken);
                return new NoContentResult();
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { Message = exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}