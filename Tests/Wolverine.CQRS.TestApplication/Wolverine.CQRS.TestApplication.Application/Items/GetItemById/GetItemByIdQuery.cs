using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItemById
{
    public class GetItemByIdQuery : IQuery
    {
        public GetItemByIdQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }
}