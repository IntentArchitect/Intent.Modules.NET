using Grpc.Core;

namespace GrpcServer.Api.Services
{
    public class ProductsGrpcService : Products.ProductsBase
    {
        public override Task<CreateProductReply> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CreateProductReply
            {
                Value = "Hello world!"
            });
        }
    }
}