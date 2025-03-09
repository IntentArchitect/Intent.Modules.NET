using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.CreateComplexProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class CreateComplexProductCommand
    {
        public Application.Products.CreateComplexProduct.CreateComplexProductCommand ToContract()
        {
            return new Application.Products.CreateComplexProduct.CreateComplexProductCommand(name: Name, typeTestField: TypeTestField.ToContract());
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static CreateComplexProductCommand? Create(Application.Products.CreateComplexProduct.CreateComplexProductCommand? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new CreateComplexProductCommand
            {
                Name = contract.Name,
                TypeTestField = TypeTestDto.Create(contract.TypeTestField)
            };

            return message;
        }
    }
}