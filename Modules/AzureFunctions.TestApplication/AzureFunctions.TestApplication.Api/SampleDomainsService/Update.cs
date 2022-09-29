using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Behaviours;
using AzureFunctions.TestApplication.Application.Interfaces;
using AzureFunctions.TestApplication.Application.SampleDomains;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api.SampleDomainsService
{
    public class Update
    {
        private readonly ValidationBehaviour<SampleDomainUpdateDTO> _validation;
        private readonly ISampleDomainsService _appService;
        private readonly IUnitOfWork _unitOfWork;
        public Update(
            ValidationBehaviour<SampleDomainUpdateDTO> validation,
            ISampleDomainsService appService,
            IUnitOfWork unitOfWork)
        {
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("SampleDomainsService-Update")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "sampledomain/{id}")] HttpRequest req,
            Guid id,
            ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var dto = JsonConvert.DeserializeObject<SampleDomainUpdateDTO>(requestBody);
                await _validation.Handle(dto, default);
                await _appService.Update(id, dto);
                await _unitOfWork.SaveChangesAsync();
                return new NoContentResult();
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