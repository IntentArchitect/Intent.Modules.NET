using System.Net;
using System.Transactions;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
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
    public class FunctionWithIgnoreInApi
    {
        private readonly IUnitOfWork _unitOfWork;

        public FunctionWithIgnoreInApi(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("FunctionWithIgnoreInApi")]
        [OpenApiOperation("FunctionWithIgnoreInApi", tags: new[] { "FunctionWithIgnoreInApi" }, Description = "Function with ignore in api")]
        [OpenApiIgnoreAttribute]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "function-with-ignore-in-api")] HttpRequest req,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // IntentIgnore
                return null;
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