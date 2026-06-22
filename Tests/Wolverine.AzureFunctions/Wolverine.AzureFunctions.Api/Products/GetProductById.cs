using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Wolverine.AzureFunctions.Application.Products;
using Wolverine.AzureFunctions.Application.Products.GetProductById;
using Wolverine.AzureFunctions.Domain.Common.Exceptions;
using Wolverine.AzureFunctions.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace Wolverine.AzureFunctions.Api.Products
{
    public class GetProductById
    {
        private readonly IMessageBus _sender;

        public GetProductById(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [Function("GetProductById")]
        [OpenApiOperation("GetProductByIdQuery", tags: new[] { "Products" }, Description = "Get product by id query")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProductDto))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.InvokeAsync<ProductDto>(new GetProductByIdQuery(id: id), cancellationToken);
                return result != null ? new OkObjectResult(result) : new NotFoundResult();
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