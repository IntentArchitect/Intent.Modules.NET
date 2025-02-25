using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Application.Products.ApplyTagsProduct;
using GrpcServer.Application.Products.CreateProduct;
using GrpcServer.GrpcServices;
using MediatR;

namespace GrpcServer.Api.Services
{
    public class ProductsGrpcService : ProductsService.ProductsServiceBase
    {
        private readonly ISender _mediator;

        public ProductsGrpcService(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<Empty> ApplyTagsProduct(ApplyTagsProductRequest request, ServerCallContext context)
        {
            var command = new ApplyTagsProductCommand(Guid.Parse(request.Id), request.TagNames.ToList());
            await _mediator.Send(command, context.CancellationToken);

            return new Empty();
        }

        public override async Task<CreateProductReply> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var requestTypeTestField = request.TypeTestField;

            var command = new CreateProductCommand(
                name: request.Name,
                typeTestField: request.TypeTestField.FromMessage());
            var result = await _mediator.Send(command, context.CancellationToken);

            return new CreateProductReply
            {
                Value = result.ToString()
            };
        }
    }
}