using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Infrastructure.Eventing;
using SolaceSystems.Solclient.Messaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceConfiguration", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Configuration
{
    public static class SolaceConfiguration
    {
        public static void AddSolaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Solace").Get<SolaceConfig>();
            if (config == null)
            {
                throw new Exception("Unable to load / find Solace configuration in appsettings.json");
            }
            else
            {
                config.Validate();
            }

            // Initialize Solace Systems Messaging API with logging to console at Warning level
            var cfp = new ContextFactoryProperties()
            {
                SolClientLogLevel = SolLogLevel.Warning
            };
            ContextFactory.Instance.Init(cfp);
            var context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);

            // Create session properties
            var sessionProps = new SessionProperties()
            {
                Host = config.Host,
                VPNName = config.VPNName,
                UserName = config.UserName,
                Password = config.Password,
                ReconnectRetries = config.ReconnectRetries ?? -1,
                GdWithWebTransport = config.GdWithWebTransport ?? true,
                SSLValidateCertificate = config.SSLValidateCertificate ?? false,
                ProvisionTimeoutInMsecs = config.ProvisionTimeoutInMsecs ?? 10000
            };

            var session = context.CreateSession(sessionProps, HandleSessionMessageEvent, HandleSessionEvent);
            services.AddSingleton(context);
            services.AddSingleton(session);
            services.AddSingleton<DispatchResolver>();
            services.AddSingleton<MessageSerializer>();
            services.AddSingleton<MessageRegistry>();
            services.AddHostedService<SolaceConsumingService>();
            services.AddScoped(typeof(ISolaceEventDispatcher<>), typeof(SolaceEventDispatcher<>));
            services.AddScoped<IEventBus, SolaceEventBus>();
            services.AddTransient<SolaceConsumer>();
        }

        private static void HandleSessionMessageEvent(object? sender, MessageEventArgs e)
        {
            // Handle incoming messages if necessary
        }

        private static void HandleSessionEvent(object? sender, SessionEventArgs e)
        {
            // Handle session events if necessary
        }

        public class SolaceConfig
        {
            public string? Host { get; set; }
            public string? VPNName { get; set; }
            public string? UserName { get; set; }
            public string? Password { get; set; }
            public int? ReconnectRetries { get; set; }
            public bool? GdWithWebTransport { get; set; }
            public bool? SSLValidateCertificate { get; set; }
            public int? ProvisionTimeoutInMsecs { get; set; }
            public string? EnvironmentPrefix { get; set; }
            public string? Application { get; set; }

            public void Validate()
            {
                if (Host == null)
                    throw new Exception("Solace Host not configured");
                if (VPNName == null)
                    throw new Exception("Solace VPN Name not configured");
                if (UserName == null)
                    throw new Exception("Solace UserName not configured");
                if (Password == null)
                    throw new Exception("Solace Password not configured");
            }
        }
    }
}