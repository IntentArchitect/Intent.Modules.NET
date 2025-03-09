using System;
using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.DeleteProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class DeleteProductCommand
    {
        public Application.Products.DeleteProduct.DeleteProductCommand ToContract()
        {
            return new Application.Products.DeleteProduct.DeleteProductCommand(id: Guid.Parse(Id));
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DeleteProductCommand? Create(Application.Products.DeleteProduct.DeleteProductCommand? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DeleteProductCommand
            {
                Id = contract.Id.ToString()
            };

            return message;
        }
    }
}