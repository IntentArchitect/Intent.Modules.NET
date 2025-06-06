using System.Collections.Generic;

namespace AzureFunctions.AzureEventGrid.Application.Common.Eventing;

public interface ICloudEventContext
{
    IDictionary<string, object> ExtensionAttributes { get; }
}