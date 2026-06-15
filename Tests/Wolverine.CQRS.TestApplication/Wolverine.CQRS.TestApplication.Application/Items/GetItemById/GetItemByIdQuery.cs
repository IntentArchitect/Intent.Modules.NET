using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.GetItemById
{
    public class GetItemByIdQuery
    {
        public Guid Id { get; set; }
    }
}
