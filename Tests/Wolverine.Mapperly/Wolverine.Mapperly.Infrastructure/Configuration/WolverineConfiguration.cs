using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.Mapperly.Application.Common.Interfaces;
using Wolverine.Mapperly.Application.Products.GetProductById;
using Wolverine.Mapperly.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.Mapperly.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts, IConfiguration configuration)
        {
            ConfigureCqrs(opts);
        }

        private static void ConfigureCqrs(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Discovery.IncludeType<GetProductByIdHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}