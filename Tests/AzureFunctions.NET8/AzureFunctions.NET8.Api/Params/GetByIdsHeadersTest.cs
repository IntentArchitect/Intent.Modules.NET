using System.Net;
using AzureFunctions.NET8.Application.Params.GetByIdsHeadersTest;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
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
    public class GetByIdsHeadersTest
    {
        private readonly IMediator _mediator;

        public GetByIdsHeadersTest(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("Params_GetByIdsHeadersTest")]
        [OpenApiOperation("GetByIdsHeadersTest", tags: new[] { "Params" }, Description = "Get by ids headers test")]
        [OpenApiParameter(name: "ids", In = ParameterLocation.Header, Required = true, Type = typeof(IEnumerable<int>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "params/by-ids-headers-test")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var ids = AzureFunctionHelper.GetHeadersParamCollection("ids"
                    , req.Headers
                    , (string val, out int parsed) => int.TryParse(val, out parsed)).ToList();
                var result = await _mediator.Send(new Application.Params.GetByIdsHeadersTest.GetByIdsHeadersTest(ids: ids), cancellationToken);
                return result != null ? new OkObjectResult(result) : new NotFoundResult();
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