using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.NET6.Application;
using AzureFunctions.NET6.Application.Interfaces;
using AzureFunctions.NET6.Application.SampleDomains;
using AzureFunctions.NET6.Domain.Common.Exceptions;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using FluentValidation;
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
    public class MappedAzureFunction
    {
        private readonly ISampleDomainsService _appService;
        private readonly IValidationService _validator;
        private readonly IUnitOfWork _unitOfWork;

        public MappedAzureFunction(ISampleDomainsService appService, IValidationService validator, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("MappedAzureFunction")]
        [OpenApiOperation("CustomOnAzureFunction", tags: new[] { "MappedAzureFunction" }, Description = "Mapped azure function")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleMappedRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "mapped-azure-function")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var request = await AzureFunctionHelper.DeserializeJsonContentAsync<SampleMappedRequest>(req.Body, cancellationToken);
                await _validator.Handle(request, cancellationToken);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _appService.MappedAzureFunction(request, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                    return new CreatedResult(string.Empty, result);
                }
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
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