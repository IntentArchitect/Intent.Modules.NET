using System;
using Amazon.SQS;
using AwsLambdaFunction.Sqs.GroupA.Eventing.Messages;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Eventing.Messages;
using AwsLambdaFunction.Sqs.GroupB.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsConfiguration", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Infrastructure.Configuration
{
    public static class SqsConfiguration
    {
        public static IServiceCollection ConfigureSqs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAWSService<AmazonSQSClient>();

            services.AddSingleton(typeof(IAmazonSQS), sp => sp.GetRequiredService<AmazonSQSClient>());

            services.AddScoped<IEventBus, SqsEventBus>();
            services.AddSingleton<SqsMessageDispatcher>();
            services.AddSingleton<ISqsMessageDispatcher, SqsMessageDispatcher>(sp => sp.GetRequiredService<SqsMessageDispatcher>());

            services.Configure<SqsPublisherOptions>(options =>
            {
                options.AddQueue<SpecificTopicOneMessageEvent>(configuration["AwsSqs:SpecificTopic:QueueUrl"]!);
                options.AddQueue<SpecificTopicTwoMessage>(configuration["AwsSqs:SpecificTopic:QueueUrl"]!);
            });

            services.Configure<SqsSubscriptionOptions>(options =>
            {
                options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>();
                options.Add<CreateOrderCommand, IIntegrationEventHandler<CreateOrderCommand>>();
            });

            return services;
        }
    }
}