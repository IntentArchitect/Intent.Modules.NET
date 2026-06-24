using Intent.RoslynWeaver.Attributes;
using JasperFx.CodeGeneration;
using Wolverine;
using Wolverine.AzureFunctions.Application.Common.Interfaces;
using Wolverine.AzureFunctions.Application.Products.CreateProduct;
using Wolverine.AzureFunctions.Application.Products.GetProductById;
using Wolverine.AzureFunctions.Application.Products.GetProducts;
using Wolverine.AzureFunctions.Application.Products.UpdateProductPrice;
using Wolverine.AzureFunctions.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AzureFunctions.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.DisableConventionalDiscovery();

            opts.Discovery.IncludeType<CreateProductCommandHandler>();
            opts.Discovery.IncludeType<UpdateProductPriceCommandHandler>();
            opts.Discovery.IncludeType<GetProductByIdQueryHandler>();
            opts.Discovery.IncludeType<GetProductsQueryHandler>();

            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
            opts.Durability.Mode = DurabilityMode.Serverless;
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}