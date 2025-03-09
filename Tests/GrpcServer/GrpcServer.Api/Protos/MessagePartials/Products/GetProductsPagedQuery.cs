using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Products.GetProductsPaged;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Products
{
    public partial class GetProductsPagedQuery
    {
        public Application.Products.GetProductsPaged.GetProductsPagedQuery ToContract()
        {
            return new Application.Products.GetProductsPaged.GetProductsPagedQuery(pageNo: PageNo, pageSize: PageSize, orderBy: OrderBy);
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static GetProductsPagedQuery? Create(Application.Products.GetProductsPaged.GetProductsPagedQuery? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new GetProductsPagedQuery
            {
                PageNo = contract.PageNo,
                PageSize = contract.PageSize,
                OrderBy = contract.OrderBy
            };

            return message;
        }
    }
}