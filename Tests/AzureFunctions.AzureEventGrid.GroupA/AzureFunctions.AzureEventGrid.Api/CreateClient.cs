using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AzureEventGrid.Application.CreateClient;
using AzureFunctions.AzureEventGrid.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureEventGrid.Api
{
    public class CreateClient
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateClient(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("CreateClient")]
        [OpenApiOperation("CreateClientCommand", tags: new[] { "AzureFunctionsAzureEventGridServices" }, Description = "Create client command")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateClientCommand))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Created)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "azure-functions-azure-event-grid-services")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            var command = await AzureFunctionHelper.DeserializeJsonContentAsync<CreateClientCommand>(req.Body, cancellationToken);
            await _mediator.Send(command, cancellationToken);
            return new CreatedResult(string.Empty, null);
        }
    }
}