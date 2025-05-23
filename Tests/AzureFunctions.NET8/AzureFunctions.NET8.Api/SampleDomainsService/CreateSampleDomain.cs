using System.Net;
using System.Text.Json;
using System.Transactions;
using AzureFunctions.NET8.Application;
using AzureFunctions.NET8.Application.Interfaces;
using AzureFunctions.NET8.Application.SampleDomains;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.SampleDomainsService
{
    public class CreateSampleDomain
    {
        private readonly ISampleDomainsService _appService;
        private readonly IValidationService _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSampleDomain(ISampleDomainsService appService, IValidationService validator, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("SampleDomainsService_CreateSampleDomain")]
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
                var dto = await AzureFunctionHelper.DeserializeJsonContentAsync<SampleDomainCreateDto>(req.Body, cancellationToken);
                await _validator.Handle(dto, cancellationToken);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _appService.CreateSampleDomain(dto, cancellationToken);
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