using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.Common.Pagination;
using AzureFunctions.NET6.Application.Customers;
using AzureFunctions.NET6.Application.Customers.GetCustomersPaged;
using AzureFunctions.NET6.Domain.Common.Exceptions;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.Customers
{
    public class GetCustomersPaged
    {
        private readonly IMediator _mediator;

        public GetCustomersPaged(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("Customers_GetCustomersPaged")]
        [OpenApiOperation("GetCustomersPagedQuery", tags: new[] { "CustomersPaged" }, Description = "Get customers paged query")]
        [OpenApiParameter(name: "pageNo", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedResult<CustomerDto>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers-paged")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                int pageNo = AzureFunctionHelper.GetQueryParam("pageNo", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                int pageSize = AzureFunctionHelper.GetQueryParam("pageSize", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                var result = await _mediator.Send(new GetCustomersPagedQuery(pageNo: pageNo, pageSize: pageSize), cancellationToken);
                return result != null ? new OkObjectResult(result) : new NotFoundResult();
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
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