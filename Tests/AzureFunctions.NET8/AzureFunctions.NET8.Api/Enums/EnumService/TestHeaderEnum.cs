using System.Net;
using System.Transactions;
using AzureFunctions.NET8.Application.Enums;
using AzureFunctions.NET8.Application.Interfaces.Enums;
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

namespace AzureFunctions.NET8.Api.Enums.EnumService
{
    public class TestHeaderEnum
    {
        private readonly IEnumService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public TestHeaderEnum(IEnumService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Enums_EnumService_TestHeaderEnum")]
        [OpenApiOperation("TestHeaderEnum", tags: new[] { "Enum" }, Description = "Test header enum")]
        [OpenApiParameter(name: "testEnum", In = ParameterLocation.Header, Required = true, Type = typeof(Company))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "enum/test-header-enum")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                Company testEnum = AzureFunctionHelper.GetHeadersParam("testEnum", req.Headers, (string val, out Company parsed) => Company.TryParse(val, out parsed));

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.TestHeaderEnum(testEnum, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                    return new CreatedResult(string.Empty, null);
                }
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { exception.Message });
            }
        }
    }
}