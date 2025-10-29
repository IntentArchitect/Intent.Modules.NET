using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Lambda.Functions.Sqs.LambdaFunctionConsumer", Version = "1.0")]

namespace Aws.Sqs.GrpB.Api
{
    public class CreateOrderCommandConsumer
    {
        private readonly ILogger<CreateOrderCommandConsumer> _logger;
        private readonly ISqsMessageDispatcher _dispatcher;
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;

        public CreateOrderCommandConsumer(ILogger<CreateOrderCommandConsumer> logger,
            ISqsMessageDispatcher dispatcher,
            IEventBus eventBus,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [LambdaFunction]
        public async Task ProcessAsync(SQSEvent sqsEvent, ILambdaContext context)
        {
            // AWSLambda0107: passing CancellationToken parameters is not supported; use CancellationToken.None instead.
            var cancellationToken = CancellationToken.None;

            foreach (var record in sqsEvent.Records)
            {
                try
                {
                    await _dispatcher.DispatchAsync(_serviceProvider, record, cancellationToken);
                    await _eventBus.FlushAllAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing CreateOrderCommandConsumer message with ID {MessageId}", record.MessageId);
                    throw;
                }
            }
        }
    }
}