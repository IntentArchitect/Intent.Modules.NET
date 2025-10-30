using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using AwsLambdaFunction.Sqs.GroupA.Api.Helpers;
using AwsLambdaFunction.Sqs.GroupA.Application.CreateClient;
using AwsLambdaFunction.Sqs.GroupA.Application.CreateOrder;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace Lambda
{
    public class DefaultFunctions
    {
        private readonly ILogger<DefaultFunctions> _logger;
        private readonly ISender _mediator;

        public DefaultFunctions(ILogger<DefaultFunctions> logger, ISender mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/client")]
        public async Task<IHttpResult> CreateClientAsync([FromBody] CreateClientCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _mediator.Send(command, cancellationToken);
                return HttpResults.Created();
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/order")]
        public async Task<IHttpResult> CreateOrderAsync([FromBody] CreateOrderCommand command)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _mediator.Send(command, cancellationToken);
                return HttpResults.Created();
            }, _logger);
        }
    }
}