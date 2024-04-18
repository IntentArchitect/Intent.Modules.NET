using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Params.GetByIdsQueryTest;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
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

namespace AzureFunctions.TestApplication.Api.Params
{
    public class GetByIdsQueryTest
    {
        private readonly IMediator _mediator;

        public GetByIdsQueryTest(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("Params_GetByIdsQueryTest")]
        [OpenApiOperation("GetByIdsQueryTest", tags: new[] { "Params" }, Description = "Get by ids query test")]
        [OpenApiParameter(name: "ids", In = ParameterLocation.Query, Required = true, Type = typeof(IEnumerable<int>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "params/by-ids-query-test")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var ids = AzureFunctionHelper.GetQueryParamCollection("ids"
                    , req.Query
                    , (string val, out int parsed) => int.TryParse(val, out parsed)).ToList();
                var result = await _mediator.Send(new Application.Params.GetByIdsQueryTest.GetByIdsQueryTest(ids: ids), cancellationToken);
                return result != null ? new OkObjectResult(result) : new NotFoundResult();
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