using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class CreateSampleDomain
    {
        private readonly ISampleDomainsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSampleDomain(ISampleDomainsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CreateSampleDomain")]
        [OpenApiOperation("CreateSampleDomain", tags: new[] { "SampleDomains" }, Description = "Create sample domain")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleDomainCreateDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sample-domains")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var dto = JsonConvert.DeserializeObject<SampleDomainCreateDto>(requestBody);
                var result = await _appService.CreateSampleDomain(dto);
                await _unitOfWork.SaveChangesAsync();
                return new CreatedResult(string.Empty, result);
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