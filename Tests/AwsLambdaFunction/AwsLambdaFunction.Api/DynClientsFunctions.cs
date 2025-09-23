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
using AwsLambdaFunction.Application.DynClients.CreateDynClient;
using AwsLambdaFunction.Application.DynClients.DeleteDynClient;
using AwsLambdaFunction.Application.DynClients.GetDynClientById;
using AwsLambdaFunction.Application.DynClients.GetDynClients;
using AwsLambdaFunction.Application.DynClients.UpdateDynClient;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace AwsLambdaFunction.Api
{
    public class DynClientsFunctions
    {
        private readonly ISender _mediator;

        public DynClientsFunctions(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/dyn-clients")]
        public async Task<IHttpResult> CreateDynClientAsync([FromBody] CreateDynClientCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(command, cancellationToken);
                return HttpResults.Created($"/api/dyn-clients/{Uri.EscapeDataString(result.ToString())}", new JsonResponse<string>(result));
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Delete, "/api/dyn-clients/{id}")]
        public async Task<IHttpResult> DeleteDynClientAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _mediator.Send(new DeleteDynClientCommand(id: id), cancellationToken);
                return HttpResults.Ok();
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/api/dyn-clients/{id}")]
        public async Task<IHttpResult> UpdateDynClientAsync(string id, [FromBody] UpdateDynClientCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                if (id != command.Id)
                {
                    return HttpResults.BadRequest();
                }

                await _mediator.Send(command, cancellationToken);
                return HttpResults.NewResult(HttpStatusCode.NoContent);
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/dyn-clients/{id}")]
        public async Task<IHttpResult> GetDynClientByIdAsync(string id)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetDynClientByIdQuery(id: id), cancellationToken);
                return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
            });
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/api/dyn-clients")]
        public async Task<IHttpResult> GetDynClientsAsync()
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                var result = await _mediator.Send(new GetDynClientsQuery(), cancellationToken);
                return HttpResults.Ok(result);
            });
        }
    }
}