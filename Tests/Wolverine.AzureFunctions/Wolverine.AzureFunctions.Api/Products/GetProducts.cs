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
using Wolverine.AzureFunctions.Application.Products.GetProducts;
using Wolverine.AzureFunctions.Domain.Common.Exceptions;
using Wolverine.AzureFunctions.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace Wolverine.AzureFunctions.Api.Products
{
    public class GetProducts
    {
        private readonly IMessageBus _sender;

        public GetProducts(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [Function("GetProducts")]
        [OpenApiOperation("GetProductsQuery", tags: new[] { "Products" }, Description = "Get products query")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ProductDto>))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.InvokeAsync<List<ProductDto>>(new GetProductsQuery(), cancellationToken);
                return new OkObjectResult(result);
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