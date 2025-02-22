//using Google.Protobuf.WellKnownTypes;
//using Grpc.Core;
//using GrpcServer.Application;
//using GrpcServer.Application.Products.ApplyTagsProduct;
//using GrpcServer.Application.Products.CreateProduct;
//using GrpcServer.GrpcServices;
//using MediatR;

//namespace GrpcServer.Api.Services
//{
//    public class ProductsGrpcService : ProductsService.ProductsServiceBase
//    {
//        private readonly ISender _mediator;

//        public ProductsGrpcService(ISender mediator)
//        {
//            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
//        }

//        public override async Task<Empty> ApplyTagsProduct(ApplyTagsProductRequest request, ServerCallContext context)
//        {
//            var command = new ApplyTagsProductCommand(Guid.Parse(request.Id), request.TagNames.ToList());
//            await _mediator.Send(command, context.CancellationToken);

//            return new Empty();
//        }

//        public override async Task<CreateProductReply> CreateProduct(CreateProductRequest request, ServerCallContext context)
//        {
//            var requestTypeTestField = request.TypeTestField;

//            var command = new CreateProductCommand(
//                name: request.Name,
//                typeTestField: MapToTypeTestDto(requestTypeTestField));
//            var result = await _mediator.Send(command, context.CancellationToken);

//            return new CreateProductReply
//            {
//                Value = result.ToString()
//            };
//        }

//        private static Application.TypeTestDto MapToTypeTestDto(GrpcServices.TypeTestDto requestTypeTestField)
//        {
//            return new Application.TypeTestDto
//            {
//                BinaryField = requestTypeTestField.BinaryField.ToByteArray(),
//                BinaryFieldCollection = requestTypeTestField.BinaryFieldCollection.Select(x => x.ToByteArray()).ToList(),
//                BinaryFieldNullable = requestTypeTestField.BinaryField?.ToByteArray(),
//                BinaryFieldNullableCollection = requestTypeTestField.BinaryFieldCollection?.Select(x => x.ToByteArray()).ToList(),
//                BoolField = requestTypeTestField.BoolField,
//                BoolFieldCollection = requestTypeTestField.BoolFieldCollection.ToList(),
//                BoolFieldNullable = requestTypeTestField.BoolFieldNullable,
//                BoolFieldNullableCollection = requestTypeTestField.BoolFieldNullableCollection?.Items?.ToList(),
//                ByteField = 0,
//                ByteFieldCollection = null,
//                ByteFieldNullable = null,
//                ByteFieldNullableCollection = null,
//                CharField = '\0',
//                CharFieldCollection = null,
//                CharFieldNullable = null,
//                CharFieldNullableCollection = null,
//                ComplexTypeField = null,
//                ComplexTypeFieldCollection = null,
//                ComplexTypeFieldNullable = null,
//                ComplexTypeFieldNullableCollection = null,
//                DateOnlyField = default,
//                DateOnlyFieldCollection = null,
//                DateOnlyFieldNullable = null,
//                DateOnlyFieldNullableCollection = null,
//                DateTimeField = default,
//                DateTimeFieldCollection = null,
//                DateTimeFieldNullable = null,
//                DateTimeFieldNullableCollection = null,
//                DateTimeOffsetField = default,
//                DateTimeOffsetFieldCollection = null,
//                DateTimeOffsetFieldNullable = null,
//                DateTimeOffsetFieldNullableCollection = null,
//                DecimalField = 0,
//                DecimalFieldCollection = null,
//                DecimalFieldNullable = null,
//                DecimalFieldNullableCollection = null,
//                DictionaryField = null,
//                DictionaryFieldCollection = null,
//                DictionaryFieldNullable = null,
//                DictionaryFieldNullableCollection = null,
//                DoubleField = 0,
//                DoubleFieldCollection = null,
//                DoubleFieldNullable = null,
//                DoubleFieldNullableCollection = null,
//                FloatField = 0,
//                FloatFieldCollection = null,
//                FloatFieldNullable = null,
//                FloatFieldNullableCollection = null,
//                GuidField = default,
//                GuidFieldCollection = null,
//                GuidFieldNullable = null,
//                GuidFieldNullableCollection = null,
//                IntField = 0,
//                IntFieldCollection = null,
//                IntFieldNullable = null,
//                IntFieldNullableCollection = null,
//                LongField = 0,
//                LongFieldCollection = null,
//                LongFieldNullable = null,
//                LongFieldNullableCollection = null,
//                ObjectField = null,
//                ObjectFieldCollection = null,
//                ObjectFieldNullable = null,
//                ObjectFieldNullableCollection = null,
//                PagedResultField = null,
//                PagedResultFieldNullable = null,
//                ShortField = 0,
//                ShortFieldCollection = null,
//                ShortFieldNullable = null,
//                ShortFieldNullableCollection = null,
//                StringField = null,
//                StringFieldCollection = null,
//                StringFieldNullable = null,
//                StringFieldNullableCollection = null,
//                TimeSpanField = default,
//                TimeSpanFieldCollection = null,
//                TimeSpanFieldNullable = null,
//                TimeSpanFieldNullableCollection = null
//            };
//        }
//    }
//}