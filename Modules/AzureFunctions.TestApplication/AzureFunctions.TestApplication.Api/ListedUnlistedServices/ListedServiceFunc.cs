using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api.ListedUnlistedServices
{
    public class ListedServiceFunc
    {
        private readonly IListedUnlistedServicesService _appService;
        private readonly IUnitOfWork _unitOfWork;
        public ListedServiceFunc(
            IListedUnlistedServicesService appService,
            IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("ListedUnlistedServices-ListedServiceFunc")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "listedunlistedservices")] HttpRequest req,
            ILogger log)
        {
            try
            {

                await _appService.ListedServiceFunc(param);
                await _unitOfWork.SaveChangesAsync();
                return new CreatedResult(string.Empty, null);
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}