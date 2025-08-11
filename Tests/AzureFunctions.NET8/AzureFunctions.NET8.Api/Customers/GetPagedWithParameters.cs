using System.Net;
using AzureFunctions.NET8.Application.Common.Pagination;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Customers.GetPagedWithParameters;
using AzureFunctions.NET8.Domain.Common.Exceptions;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Customers
{
    public class GetPagedWithParameters
    {
        private readonly IMediator _mediator;

        public GetPagedWithParameters(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("Customers_GetPagedWithParameters")]
        [OpenApiOperation("GetPagedWithParameters", tags: new[] { "Customers" }, Description = "Get paged with parameters")]
        [OpenApiParameter(name: "pageNo", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "pageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter(name: "searchCriteria", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedResult<CustomerDto>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/paged-with-parameters/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                int pageNo = AzureFunctionHelper.GetQueryParam("pageNo", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                int pageSize = AzureFunctionHelper.GetQueryParam("pageSize", req.Query, (string val, out int parsed) => int.TryParse(val, out parsed));
                string searchCriteria = req.Query["searchCriteria"];
                var result = await _mediator.Send(new Application.Customers.GetPagedWithParameters.GetPagedWithParameters(pageNo: pageNo, pageSize: pageSize, searchCriteria: searchCriteria, id: id), cancellationToken);
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