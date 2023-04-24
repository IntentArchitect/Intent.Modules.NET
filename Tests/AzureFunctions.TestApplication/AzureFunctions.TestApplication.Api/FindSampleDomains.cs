using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class FindSampleDomains
    {
        private readonly ISampleDomainsService _appService;

        public FindSampleDomains(ISampleDomainsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [FunctionName("FindSampleDomains")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/sampledomains/sample-domains")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _appService.FindSampleDomains();
                return new OkObjectResult(result);
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}