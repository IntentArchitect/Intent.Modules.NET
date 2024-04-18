using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Pagination;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.SampleDomainsService
{
    public class FindSampleDomainsPaged
    {
        private readonly ISampleDomainsService _appService;

        public FindSampleDomainsPaged(ISampleDomainsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [FunctionName("SampleDomainsService_FindSampleDomainsPaged")]
        [OpenApiOperation("FindSampleDomainsPaged", tags: new[] { "SampleDomainsPaged" }, Description = "Find sample domains paged")]
        [OpenApiParameter(name: "pageNo", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedResult<SampleDomainDto>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sample-domains-paged")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                int pageNo = AzureFunctionHelper.GetQueryParam("pageNo", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                int pageSize = AzureFunctionHelper.GetQueryParam("pageSize", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                var result = await _appService.FindSampleDomainsPaged(pageNo, pageSize, cancellationToken);
                return new OkObjectResult(result);
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