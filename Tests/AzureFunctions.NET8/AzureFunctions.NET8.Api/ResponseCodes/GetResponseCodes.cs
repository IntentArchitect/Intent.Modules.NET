using System.Net;
using AzureFunctions.NET8.Application.ResponseCodes;
using AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodes;
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

namespace AzureFunctions.NET8.Api.ResponseCodes
{
    public class GetResponseCodes
    {
        private readonly IMediator _mediator;

        public GetResponseCodes(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("ResponseCodes_GetResponseCodes")]
        [OpenApiOperation("GetResponseCodesQuery", tags: new[] { "ResponseCodes" }, Description = "Get response codes query")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.PartialContent, contentType: "application/json", bodyType: typeof(List<ResponseCodeDto>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "response-codes")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new GetResponseCodesQuery(), cancellationToken);
                return new ObjectResult(result) { StatusCode = 206 };
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