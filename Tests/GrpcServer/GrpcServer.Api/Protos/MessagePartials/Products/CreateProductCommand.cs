using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.Products.CreateProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class CreateProductCommand
    {
        public Application.Products.CreateProduct.CreateProductCommand ToContract()
        {
            return new Application.Products.CreateProduct.CreateProductCommand(name: Name, strings: Strings?.Items.ToList());
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static CreateProductCommand? Create(Application.Products.CreateProduct.CreateProductCommand? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new CreateProductCommand
            {
                Name = contract.Name,
                Strings = ListOfString.Create(contract.Strings)
            };

            return message;
        }
    }
}