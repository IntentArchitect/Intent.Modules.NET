using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using AwsLambdaFunction.Sqs.GroupB.Api.Helpers;
using AwsLambdaFunction.Sqs.GroupB.Application;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Validation;
using AwsLambdaFunction.Sqs.GroupB.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate", Version = "1.0")]

namespace Lambda
{
    public class SpecificChannelFunctions
    {
        private readonly ILogger<SpecificChannelFunctions> _logger;
        private readonly ISpecificChannelService _appService;
        private readonly IValidationService _validationService;
        private readonly IEventBus _eventBus;

        public SpecificChannelFunctions(ILogger<SpecificChannelFunctions> logger,
            ISpecificChannelService appService,
            IValidationService validationService,
            IEventBus eventBus)
        {
            _logger = logger;
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/specific-channel/specific-channel/send-specific-topic-one")]
        public async Task<IHttpResult> SendSpecificTopicOneAsync([FromBody] PayloadDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            await _eventBus.FlushAllAsync(cancellationToken);
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);
                await _appService.SendSpecificTopicOne(dto, cancellationToken);
                return HttpResults.Created();
            }, _logger);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/api/specific-channel/specific-channel/send-specific-topic-two")]
        public async Task<IHttpResult> SendSpecificTopicTwoAsync([FromBody] PayloadDto dto)
        {
            // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
            var cancellationToken = CancellationToken.None;
            await _eventBus.FlushAllAsync(cancellationToken);
            return await ExceptionHandlerHelper.ExecuteAsync(async () =>
            {
                await _validationService.Handle(dto, cancellationToken);
                await _appService.SendSpecificTopicTwo(dto, cancellationToken);
                return HttpResults.Created();
            }, _logger);
        }
    }
}