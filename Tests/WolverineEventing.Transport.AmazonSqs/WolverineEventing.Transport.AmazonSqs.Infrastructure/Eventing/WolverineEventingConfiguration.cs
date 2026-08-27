using Amazon;
using Amazon.Runtime;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AmazonSns;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;
using WolverineEventing.Transport.AmazonSqs.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineEventingConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.AmazonSqs.Infrastructure.Eventing
{
    public static class WolverineEventingConfiguration
    {
        public static void ConfigureAmazonSqs(WolverineOptions opts, IConfiguration configuration)
        {
            const string section = "Wolverine:AmazonSqs";
            const string key = "Region";
            var region = configuration[$"{section}:{key}"];

            if (string.IsNullOrEmpty(region))
            {
                throw new System.InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Amazon SQS.");
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

            opts.PublishMessage<OrderCreatedEvent>().ToSnsTopic("order-created-event");

            ApplyErrorHandlingPolicy(opts, configuration);
        }

        public static void ApplyErrorHandlingPolicy(WolverineOptions opts, IConfiguration configuration)
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

        public static System.TimeSpan[] ParseDelays(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
.Select(TimeSpan.Parse)
.ToArray();
        }
    }
}