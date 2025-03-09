using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.Products.ApplyTagsProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class ApplyTagsProductCommand
    {
        public Application.Products.ApplyTagsProduct.ApplyTagsProductCommand ToContract()
        {
            return new Application.Products.ApplyTagsProduct.ApplyTagsProductCommand(id: Guid.Parse(Id), tagNames: TagNames.ToList());
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ApplyTagsProductCommand? Create(Application.Products.ApplyTagsProduct.ApplyTagsProductCommand? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ApplyTagsProductCommand
            {
                Id = contract.Id.ToString()
            };

            message.TagNames.AddRange(contract.TagNames);
            return message;
        }
    }
}