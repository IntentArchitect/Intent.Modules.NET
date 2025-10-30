using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using AwsLambdaFunction.Api.Helpers;
using AwsLambdaFunction.Api.ResponseTypes;
using AwsLambdaFunction.Application.EfClients.CreateEfClient;
using AwsLambdaFunction.Application.EfClients.DeleteEfClient;
using AwsLambdaFunction.Application.EfClients.GetEfClientById;
using AwsLambdaFunction.Application.EfClients.GetEfClients;
using AwsLambdaFunction.Application.EfClients.UpdateEfClient;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace Lambda
{
    public class EfClientsFunctions
    {
        private readonly ILogger<EfClientsFunctions> _logger;
        private readonly ISender _mediator;

        public EfClientsFunctions(ILogger<EfClientsFunctions> logger, ISender mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/ef-clients")]
        public async Task<IHttpResult> CreateEfClientAsync([FromBody] CreateEfClientCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(command, cancellationToken);
                return HttpResults.Created($"/api/ef-clients/{Uri.EscapeDataString(result.ToString())}", new JsonResponse<Guid>(result));
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Delete, "/api/ef-clients/{id}")]
        public async Task<IHttpResult> DeleteEfClientAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                await _mediator.Send(new DeleteEfClientCommand(idGuid), cancellationToken);
                return HttpResults.Ok();
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/api/ef-clients/{id}")]
        public async Task<IHttpResult> UpdateEfClientAsync(string id, [FromBody] UpdateEfClientCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
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

                await _mediator.Send(command, cancellationToken);
                return HttpResults.NewResult(HttpStatusCode.NoContent);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/ef-clients/{id}")]
        public async Task<IHttpResult> GetEfClientByIdAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                // Workaround by accepting string parameters and converting to Guid here.
                if (!Guid.TryParse(id, out var idGuid))
                {
                    return HttpResults.BadRequest($"Invalid format for id: {id}");
                }

                var result = await _mediator.Send(new GetEfClientByIdQuery(idGuid), cancellationToken);
                return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/ef-clients")]
        public async Task<IHttpResult> GetEfClientsAsync()
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetEfClientsQuery(), cancellationToken);
                return HttpResults.Ok(result);
            }, _logger);
        }
    }
}