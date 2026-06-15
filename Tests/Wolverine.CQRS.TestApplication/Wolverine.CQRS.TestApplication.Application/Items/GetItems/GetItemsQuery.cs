using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItems
{
    public class GetItemsQuery : IQuery
    {
    }
}