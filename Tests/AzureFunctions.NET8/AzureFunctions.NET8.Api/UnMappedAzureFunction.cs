using System.Net;
using System.Text.Json;
using System.Transactions;
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

namespace AzureFunctions.NET8.Api
{
    public class UnMappedAzureFunction
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnMappedAzureFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("UnMappedAzureFunction")]
        [OpenApiOperation("UnMappedAzureFunction", tags: new[] { "Unmappedazurefunction" }, Description = "Hi There")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SampleDomainDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "unmappedazurefunction")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<SampleDomainDto>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
                //IntentIgnore
                return new NoContentResult();
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