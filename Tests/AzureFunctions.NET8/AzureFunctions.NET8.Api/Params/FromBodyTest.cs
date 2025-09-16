using System.Net;
using System.Text.Json;
using AzureFunctions.NET8.Application.Params.FromBodyTest;
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

namespace AzureFunctions.NET8.Api.Params
{
    public class FromBodyTest
    {
        private readonly IMediator _mediator;

        public FromBodyTest(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("Params_FromBodyTest")]
        [OpenApiOperation("FromBodyTestCommand", tags: new[] { "FromBodyTest" }, Description = "From body test command")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(IEnumerable<int>))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Created)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "from-body-test")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var ids = await AzureFunctionHelper.DeserializeJsonContentAsync<List<int>>(req.Body, cancellationToken);
                await _mediator.Send(new FromBodyTestCommand(ids: ids), cancellationToken);
                return new CreatedResult(string.Empty, null);
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { exception.Message });
            }
            catch (JsonException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
        }
    }
}