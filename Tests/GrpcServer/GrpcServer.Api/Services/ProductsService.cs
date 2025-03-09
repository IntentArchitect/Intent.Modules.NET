using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Api.Protos.Messages;
using GrpcServer.Api.Protos.Messages.Products;
using GrpcServer.Api.Protos.Services.Products;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.CqrsService", Version = "1.0")]

namespace GrpcServer.Api.Services
{
    [Authorize]
    public class ProductsService : Products.ProductsBase
    {
        private readonly ISender _mediator;

        public ProductsService(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<Empty> ApplyTagsProduct(ApplyTagsProductCommand request, ServerCallContext context)
        {
            await _mediator.Send(request.ToContract(), context.CancellationToken);
            return new Empty();
        }

        [AllowAnonymous]
        public override async Task<StringValue> CreateComplexProduct(
            CreateComplexProductCommand request,
            ServerCallContext context)
        {
            var result = await _mediator.Send(request.ToContract(), context.CancellationToken);
            return new StringValue { Value = result.ToString() };
        }

        [Authorize]
        public override async Task<StringValue> CreateProduct(CreateProductCommand request, ServerCallContext context)
        {
            var result = await _mediator.Send(request.ToContract(), context.CancellationToken);
            return new StringValue { Value = result.ToString() };
        }

        public override async Task<Empty> DeleteProduct(DeleteProductCommand request, ServerCallContext context)
        {
            await _mediator.Send(request.ToContract(), context.CancellationToken);
            return new Empty();
        }

        [Authorize(Roles = "SomeRole")]
        public override async Task<Empty> UpdateProduct(UpdateProductCommand request, ServerCallContext context)
        {
            await _mediator.Send(request.ToContract(), context.CancellationToken);
            return new Empty();
        }

        public override async Task<ProductDto> GetProductById(GetProductByIdQuery request, ServerCallContext context)
        {
            var result = await _mediator.Send(request.ToContract(), context.CancellationToken);
            return ProductDto.Create(result);
        }

        [Authorize]
        public override async Task<PagedResultOfProductDto> GetProductsPaged(
            GetProductsPagedQuery request,
            ServerCallContext context)
        {
            var result = await _mediator.Send(request.ToContract(), context.CancellationToken);
            return PagedResultOfProductDto.Create(result);
        }

        public override async Task<ListOfProductDto> GetProducts(GetProductsQuery request, ServerCallContext context)
        {
            var result = await _mediator.Send(request.ToContract(), context.CancellationToken);
            return ListOfProductDto.Create(result);
        }
    }
}