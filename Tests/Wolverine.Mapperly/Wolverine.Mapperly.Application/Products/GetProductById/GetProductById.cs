using Intent.RoslynWeaver.Attributes;
using Wolverine.Mapperly.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.Mapperly.Application.Products.GetProductById
{
    public class GetProductById : IQuery
    {
        public GetProductById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}