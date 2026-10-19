using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AppSettings.Application.Interfaces;
using AzureFunctions.AppSettings.Domain.Common.Exceptions;
using AzureFunctions.AppSettings.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AppSettings.Api.TriggerAppSettingsService
{
    public class GetStatus
    {
        private readonly ITriggerAppSettingsService _appService;

        public GetStatus(ITriggerAppSettingsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [Function("GetStatus")]
        [OpenApiOperation("GetStatus", tags: new[] { "Status" }, Description = "Http trigger; must be unaffected by the setting.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _appService.GetStatus(cancellationToken);
                return new OkObjectResult(result);
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