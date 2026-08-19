using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using WebAndWorker.Application;
using WebAndWorker.Infrastructure;
using WebAndWorker.QuartzWorker.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.VisualStudio.Projects.ServiceWorker.ServiceWorkerProgram", Version = "1.0")]

namespace WebAndWorker.QuartzWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureApplicationSecurity(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.ConfigureQuartz(builder.Configuration);

            // Add services to the container.

            var app = builder.Build();

            app.Run();
        }
    }
}