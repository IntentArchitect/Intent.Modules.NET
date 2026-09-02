using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AspNetCore.FastEndpoints.Application.Common.Interfaces;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.ChangeProductProduct;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.CreateProduct;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.GetProductById;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.GetProducts;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.UpdateProductPrice;
using Wolverine.AspNetCore.FastEndpoints.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Infrastructure.Configuration
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
            opts.Discovery.IncludeType<ChangeProductProductCommandHandler>();
            opts.Discovery.IncludeType<CreateProductCommandHandler>();
            opts.Discovery.IncludeType<GetProductByIdQueryHandler>();
            opts.Discovery.IncludeType<GetProductsQueryHandler>();
            opts.Discovery.IncludeType<UpdateProductPriceCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}