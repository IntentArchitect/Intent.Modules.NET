using System;
using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.UpdateProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class UpdateProductCommand
    {
        public Application.Products.UpdateProduct.UpdateProductCommand ToContract()
        {
            return new Application.Products.UpdateProduct.UpdateProductCommand(name: Name, id: Guid.Parse(Id));
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static UpdateProductCommand? Create(Application.Products.UpdateProduct.UpdateProductCommand? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new UpdateProductCommand
            {
                Name = contract.Name,
                Id = contract.Id.ToString()
            };

            return message;
        }
    }
}