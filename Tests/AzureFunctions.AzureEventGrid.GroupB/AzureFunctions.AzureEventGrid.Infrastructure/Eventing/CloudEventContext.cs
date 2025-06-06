using System.Collections.Generic;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;

namespace AzureFunctions.AzureEventGrid.Infrastructure.Eventing;

public class CloudEventContext : ICloudEventContext
{
    public IDictionary<string, object> ExtensionAttributes { get; } = new Dictionary<string, object>();
}