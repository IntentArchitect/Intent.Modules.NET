using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.EventContextInterface", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.Common.Eventing;

public interface IEventContext
{
    IDictionary<string, object> AdditionalData { get; }
}