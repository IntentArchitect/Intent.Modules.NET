using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AspNetCore.Controllers.Application.ChangeProductProduct;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;
using Wolverine.AspNetCore.Controllers.Application.CreateOrder;
using Wolverine.AspNetCore.Controllers.Application.CreateProduct;
using Wolverine.AspNetCore.Controllers.Application.DeleteOrder;
using Wolverine.AspNetCore.Controllers.Application.GetOrderById;
using Wolverine.AspNetCore.Controllers.Application.GetOrders;
using Wolverine.AspNetCore.Controllers.Application.GetOrderStatistics;
using Wolverine.AspNetCore.Controllers.Application.GetProductById;
using Wolverine.AspNetCore.Controllers.Application.GetProducts;
using Wolverine.AspNetCore.Controllers.Application.New;
using Wolverine.AspNetCore.Controllers.Application.PlaceOrder;
using Wolverine.AspNetCore.Controllers.Application.UpdateOrder;
using Wolverine.AspNetCore.Controllers.Application.UpdateProductPrice;
using Wolverine.AspNetCore.Controllers.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Wolverine.Common.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Infrastructure.Configuration
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
            opts.Discovery.IncludeType<CreateOrderCommandHandler>();
            opts.Discovery.IncludeType<CreateProductCommandHandler>();
            opts.Discovery.IncludeType<DeleteOrderCommandHandler>();
            opts.Discovery.IncludeType<GetOrderByIdQueryHandler>();
            opts.Discovery.IncludeType<GetOrderStatisticsQueryHandler>();
            opts.Discovery.IncludeType<GetOrdersQueryHandler>();
            opts.Discovery.IncludeType<GetProductByIdQueryHandler>();
            opts.Discovery.IncludeType<GetProductsQueryHandler>();
            opts.Discovery.IncludeType<NewCommandHandler>();
            opts.Discovery.IncludeType<PlaceOrderCommandHandler>();
            opts.Discovery.IncludeType<UpdateOrderCommandHandler>();
            opts.Discovery.IncludeType<UpdateProductPriceCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}