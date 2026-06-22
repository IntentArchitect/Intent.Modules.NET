using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Wolverine.AzureFunctions.Application.Products.UpdateProductPrice;
using Wolverine.AzureFunctions.Domain.Common.Exceptions;
using Wolverine.AzureFunctions.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace Wolverine.AzureFunctions.Api.Products
{
    public class UpdateProductPrice
    {
        private readonly IMessageBus _sender;

        public UpdateProductPrice(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [Function("UpdateProductPrice")]
        [OpenApiOperation("UpdateProductPriceCommand", tags: new[] { "Products" }, Description = "Update product price command")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateProductPriceCommand))]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "products/{id}")] HttpRequest req,
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = await AzureFunctionHelper.DeserializeJsonContentAsync<UpdateProductPriceCommand>(req.Body, cancellationToken);
                if (id != command.Id)
                {
                    return new BadRequestObjectResult(new { Message = "Supplied 'id' does not match 'Id' from body." });
                }
                await _sender.InvokeAsync(command, cancellationToken);
                return new NoContentResult();
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