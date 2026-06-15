using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.CreateItem
{
    public class CreateItemCommand
    {
        public string Name { get; set; } = null!;
    }
}
