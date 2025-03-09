using System;
using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.GetProductById;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class GetProductByIdQuery
    {
        public Application.Products.GetProductById.GetProductByIdQuery ToContract()
        {
            return new Application.Products.GetProductById.GetProductByIdQuery(id: Guid.Parse(Id));
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static GetProductByIdQuery? Create(Application.Products.GetProductById.GetProductByIdQuery? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new GetProductByIdQuery
            {
                Id = contract.Id.ToString()
            };

            return message;
        }
    }
}