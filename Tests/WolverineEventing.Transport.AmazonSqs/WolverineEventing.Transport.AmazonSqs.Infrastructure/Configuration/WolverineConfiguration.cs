using Amazon;
using Amazon.Runtime;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AmazonSns;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;
using WolverineEventing.Transport.AmazonSqs.Application.Common.Interfaces;
using WolverineEventing.Transport.AmazonSqs.Application.Orders.CreateOrder;
using WolverineEventing.Transport.AmazonSqs.Eventing.Messages;
using WolverineEventing.Transport.AmazonSqs.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureCqrs(opts);

            ConfigureEventing(opts, configuration);
        }

        private static void ConfigureCqrs(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Discovery.IncludeType<CreateOrderCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }

        private static void ConfigureEventing(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureAmazonSqsTransport(opts, configuration);

            ConfigurePublishing(opts);

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        private static void ConfigureAmazonSqsTransport(WolverineOptions opts, IConfiguration configuration)
        {
            const string section = "Wolverine:AmazonSqs";
            const string key = "Region";
            var region = configuration[$"{section}:{key}"];

            if (string.IsNullOrEmpty(region))
            {
                throw new InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Amazon SQS.");
            }

            var accessKey = configuration[$"{section}:AccessKey"];
            var secretKey = configuration[$"{section}:SecretKey"];

            var transport = opts.UseAmazonSqsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                transport.Credentials(new BasicAWSCredentials(accessKey, secretKey));
            }

            transport.AutoProvision();

            var snsTransport = opts.UseAmazonSnsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                snsTransport.Credentials(new BasicAWSCredentials(accessKey, secretKey));
            }

            snsTransport.AutoProvision();
        }

        private static void ConfigurePublishing(WolverineOptions opts)
        {
            opts.PublishMessage<OrderCreatedEvent>().ToSnsTopic("order-created-event");
        }

        private static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
        {
            var delays = ParseDelays(configuration["Wolverine:ErrorHandling:RetryWithCooldown:Delays"] ?? "00:00:01, 00:00:05, 00:00:15");

            if (delays.Length == 0)
            {
                opts.OnException<Exception>().MoveToErrorQueue();
            }
            else
            {
                opts.OnException<Exception>().RetryWithCooldown(delays).Then.MoveToErrorQueue();
            }
        }

        private static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(TimeSpan.Parse).ToArray();
        }
    }
}