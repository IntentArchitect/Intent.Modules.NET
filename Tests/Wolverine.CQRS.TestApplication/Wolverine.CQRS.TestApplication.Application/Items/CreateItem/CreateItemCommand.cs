using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items.CreateItem
{
    public class CreateItemCommand
        : ICommand
    {
        public string Name { get; set; } = null!;
    }
}
