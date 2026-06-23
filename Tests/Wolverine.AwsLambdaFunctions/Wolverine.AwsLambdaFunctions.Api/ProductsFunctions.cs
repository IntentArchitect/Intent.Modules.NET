using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.AwsLambdaFunctions.Api.Helpers;
using Wolverine.AwsLambdaFunctions.Application.Products;
using Wolverine.AwsLambdaFunctions.Application.Products.CreateProduct;
using Wolverine.AwsLambdaFunctions.Application.Products.GetProductById;
using Wolverine.AwsLambdaFunctions.Application.Products.GetProducts;
using Wolverine.AwsLambdaFunctions.Application.Products.UpdateProductPrice;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace Lambda
{
    public class ProductsFunctions
    {
        private readonly ILogger<ProductsFunctions> _logger;
        private readonly IMessageBus _sender;

        public ProductsFunctions(ILogger<ProductsFunctions> logger, IMessageBus sender)
        {
            _logger = logger;
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/products")]
        public async Task<IHttpResult> CreateProductAsync([FromBody] CreateProductCommand command)
        {
            // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _sender.InvokeAsync<Guid>(command, cancellationToken);
                return HttpResults.Created($"/api/products/{Uri.EscapeDataString(result.ToString())}", result);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/api/products/{id}")]
        public async Task<IHttpResult> UpdateProductPriceAsync(string id, [FromBody] UpdateProductPriceCommand command)
        {
            // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                if (idGuid != command.Id)
                {
                    return HttpResults.BadRequest();
                }

                await _sender.InvokeAsync(command, cancellationToken);
                return HttpResults.NewResult(HttpStatusCode.NoContent);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/products/{id}")]
        public async Task<IHttpResult> GetProductByIdAsync(string id)
        {
            // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                var result = await _sender.InvokeAsync<ProductDto>(new GetProductByIdQuery(idGuid), cancellationToken);
                return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/products")]
        public async Task<IHttpResult> GetProductsAsync()
        {
            // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _sender.InvokeAsync<List<ProductDto>>(new GetProductsQuery(), cancellationToken);
                return HttpResults.Ok(result);
            }, _logger);
        }
    }
}