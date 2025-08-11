using System.Net;
using System.Transactions;
using AzureFunctions.NET8.Application.Ignores.CommandWithIgnoreInApi;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
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

namespace AzureFunctions.NET8.Api.Ignores
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

        [Function("Ignores_CommandWithIgnoreInApi")]
        [OpenApiOperation("CommandWithIgnoreInApi", tags: new[] { "Ignores" }, Description = "Command with ignore in api")]
        [OpenApiIgnoreAttribute]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "ignores/command-with-ignore-in-api")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new Application.Ignores.CommandWithIgnoreInApi.CommandWithIgnoreInApi(), cancellationToken);
                return new NoContentResult();
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
        }
    }
}