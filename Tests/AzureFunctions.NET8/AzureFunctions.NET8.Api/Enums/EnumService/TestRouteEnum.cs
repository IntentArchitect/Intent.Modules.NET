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
    public class TestRouteEnum
    {
        private readonly IEnumService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public TestRouteEnum(IEnumService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Enums_EnumService_TestRouteEnum")]
        [OpenApiOperation("TestRouteEnum", tags: new[] { "RouteEnum" }, Description = "Test route enum")]
        [OpenApiParameter(name: "testEnum", In = ParameterLocation.Path, Required = true, Type = typeof(Company))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "route-enum/{testenum}/test-route-enum")] HttpRequest req,
            string testEnum,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testEnumEnum = AzureFunctionHelper.GetEnumParam<Company>(nameof(testEnum), testEnum);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.TestRouteEnum(testEnumEnum, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                    return new CreatedResult(string.Empty, null);
                }
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