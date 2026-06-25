using Intent.RoslynWeaver.Attributes;
using JasperFx.CodeGeneration;
using Wolverine;
using Wolverine.AwsLambdaFunctions.Application.Common.Interfaces;
using Wolverine.AwsLambdaFunctions.Application.Products.CreateProduct;
using Wolverine.AwsLambdaFunctions.Application.Products.GetProductById;
using Wolverine.AwsLambdaFunctions.Application.Products.GetProducts;
using Wolverine.AwsLambdaFunctions.Application.Products.UpdateProductPrice;
using Wolverine.AwsLambdaFunctions.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.DisableConventionalDiscovery();

            RegisterHandlers(opts);

            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
            opts.Durability.Mode = DurabilityMode.Serverless;
            ApplicationHandlerPolicy.Apply(opts);
        }

        private static void RegisterHandlers(WolverineOptions opts)
        {
            opts.Discovery.IncludeType<CreateProductCommandHandler>();
            opts.Discovery.IncludeType<UpdateProductPriceCommandHandler>();
            opts.Discovery.IncludeType<GetProductByIdQueryHandler>();
            opts.Discovery.IncludeType<GetProductsQueryHandler>();
        }
    }
}