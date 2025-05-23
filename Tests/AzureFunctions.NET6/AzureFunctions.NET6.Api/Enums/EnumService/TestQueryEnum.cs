using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.NET6.Application.Enums;
using AzureFunctions.NET6.Application.Interfaces.Enums;
using AzureFunctions.NET6.Domain.Common.Exceptions;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.Enums.EnumService
{
    public class TestQueryEnum
    {
        private readonly IEnumService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public TestQueryEnum(IEnumService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("Enums_EnumService_TestQueryEnum")]
        [OpenApiOperation("TestQueryEnum", tags: new[] { "Enum" }, Description = "Test query enum")]
        [OpenApiParameter(name: "testEnum", In = ParameterLocation.Query, Required = true, Type = typeof(Company))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "enum/test-query-enum")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                Company testEnum = AzureFunctionHelper.GetQueryParam("testEnum", req.Query, (string val, out Company parsed) => Company.TryParse(val, out parsed));

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.TestQueryEnum(testEnum, cancellationToken);
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