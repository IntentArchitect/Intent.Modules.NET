using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class UpdateSampleDomain
    {
        private readonly ISampleDomainsService _appService;

        public UpdateSampleDomain(ISampleDomainsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [FunctionName("UpdateSampleDomain")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "api/sampledomains/sample-domains/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var dto = JsonConvert.DeserializeObject<SampleDomainUpdateDto>(requestBody);
                await _appService.UpdateSampleDomain(id, dto);
                return new NoContentResult();
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}