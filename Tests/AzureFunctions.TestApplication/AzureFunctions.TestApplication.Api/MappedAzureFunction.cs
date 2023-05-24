using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Validation;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using FluentValidation;
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
    public class MappedAzureFunction
    {
        private readonly ISampleDomainsService _appService;
        private readonly IValidationService _validator;

        public MappedAzureFunction(ISampleDomainsService appService, IValidationService validator)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [FunctionName("MappedAzureFunction")]
        [OpenApiOperation("MappedAzureFunction", tags: new[] { "Newazurefunction" }, Description = "Mapped azure function")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleMappedRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "newazurefunction")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<SampleMappedRequest>(requestBody);
                await _validator.Validate(request, default);
                var result = await _appService.MappedAzureFunction(request);
                return new CreatedResult(string.Empty, result);
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}