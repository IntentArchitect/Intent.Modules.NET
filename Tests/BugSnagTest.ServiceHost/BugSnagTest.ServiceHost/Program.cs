using BugSnagTest.ServiceHost.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.WindowsServiceHost.Program", Version = "1.0")]

namespace BugSnagTest.ServiceHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureBugsnag(builder.Configuration);
            builder.Services.ConfigureQuartz(builder.Configuration);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "BugSnagTest.ServiceHost";
            });
            LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
            builder.Services.AddHostedService<WindowsBackgroundService>();

            // Add services to the container.

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.Run();
        }
    }
}