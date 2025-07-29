using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.NET6.Application.SampleDomains;
using AzureFunctions.NET6.Domain.Common.Exceptions;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api
{
    public class UnMappedAzureFunction
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnMappedAzureFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("UnMappedAzureFunction")]
        [OpenApiOperation("UnMappedAzureFunction", tags: new[] { "Unmappedazurefunction" }, Description = "Hi There")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleDomainDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "unmappedazurefunction")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var request = await AzureFunctionHelper.DeserializeJsonContentAsync<SampleDomainDto>(req.Body, cancellationToken);
                //IntentIgnore
                return new NoContentResult();
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