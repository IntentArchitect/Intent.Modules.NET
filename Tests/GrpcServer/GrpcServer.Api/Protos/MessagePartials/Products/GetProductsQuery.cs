using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.GetProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class GetProductsQuery
    {
        public Application.Products.GetProducts.GetProductsQuery ToContract()
        {
            return new Application.Products.GetProducts.GetProductsQuery();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static GetProductsQuery? Create(Application.Products.GetProducts.GetProductsQuery? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new GetProductsQuery
            {
            };

            return message;
        }
    }
}