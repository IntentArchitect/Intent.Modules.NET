using Amazon.SQS;
using CompositeMessageBus.Eventing.Messages;
using CompositeMessageBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Configuration
{
    public static class SqsConfiguration
    {
        public static IServiceCollection AddSqsConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddAWSService<AmazonSQSClient>();

            services.AddSingleton(typeof(IAmazonSQS), sp => sp.GetRequiredService<AmazonSQSClient>());

            services.AddScoped<SqsMessageBus>();
            services.AddSingleton<SqsMessageDispatcher>();
            services.AddSingleton<ISqsMessageDispatcher, SqsMessageDispatcher>(sp => sp.GetRequiredService<SqsMessageDispatcher>());
            services.Configure<SqsPublisherOptions>(options =>
            {
                options.AddQueue<MsgSqsEvent>(configuration["AwsSqs:MsgSqs:QueueUrl"]!);
            });

            registry.Register<MsgSqsEvent, SqsMessageBus>();

            return services;
        }
    }
}